using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.DTOs.PageBuilder;
using SamaniCrm.Application.Pages.Commands;
using SamaniCrm.Application.Pages.Queries;
using SamaniCrm.Core.Shared.Enums;
using SamaniCrm.Core.Shared.Interfaces;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Domain.Entities.PageBuilderEntities;
using SamaniCrm.Infrastructure.Extensions;
using SamaniCrm.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SamaniCrm.Infrastructure.Services
{
    public class PageService : IPageService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILocalizer L;



        public PageService(ApplicationDbContext context, ICurrentUserService currentUserService, ILocalizer l)
        {
            _context = context;
            _currentUserService = currentUserService;
            L = l;
        }



        public async Task<Guid> CreateOrEditMetaDataPage(CreateOrEditPageMetaDataCommand request, CancellationToken cancellationToken)
        {
            Page? page;

            if (request.Id.HasValue)
            {
                page = await _context.Pages
                    .Include(p => p.Translations)
                    .FirstOrDefaultAsync(p => p.Id == request.Id.Value, cancellationToken);

                if (page == null)
                    throw new NotFoundException("Page not found");

                // Update translations
                foreach (var item in request.Translations ?? [])
                {
                    var existingTranslation = page.Translations
                        .FirstOrDefault(t => t.Culture == item.Culture);

                    if (existingTranslation != null)
                    {
                        // Update existing translation
                        existingTranslation.Introduction = item.Introduction;
                        existingTranslation.Title = item.Title;
                        existingTranslation.Description = item.Description;
                        existingTranslation.MetaDescription = item.MetaDescription;
                        existingTranslation.MetaKeywords = item.MetaKeywords;
                    }
                    else
                    {
                        // Add new translation
                        page.Translations.Add(new PageTranslation
                        {
                            Culture = item.Culture,
                            Introduction = item.Introduction,
                            Title = item.Title,
                            Description = item.Description,
                            MetaDescription = item.MetaDescription,
                            MetaKeywords = item.MetaKeywords
                        });
                    }
                }

                // Update main page properties
                page.CoverImage = request.CoverImage;
                page.IsActive = request.IsActive;
                page.Status = request.Status ?? page.Status;
                page.Type = request.Type;
            }
            else
            {
                // New page
                page = new Page
                {
                    AuthorId = Guid.Parse(_currentUserService.UserId!),
                    CoverImage = request.CoverImage,
                    IsActive = request.IsActive,
                    IsSystem = false,
                    Status = request.Status ?? PageStatusEnum.Draft,
                    Type = request.Type,
                    Translations = (request.Translations ?? []).Select(item => new PageTranslation
                    {
                        Culture = item.Culture,
                        Introduction = item.Introduction,
                        Title = item.Title,
                        Description = item.Description,
                        MetaDescription = item.MetaDescription,
                        MetaKeywords = item.MetaKeywords,
                    }).ToList()
                };

                _context.Pages.Add(page);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return page.Id;
        }

        public async Task<Unit> DeletePage(DeletePageCommand request, CancellationToken cancellationToken)
        {
            var page = await _context.Pages
               .Include(p => p.Translations)
               .FirstOrDefaultAsync(p => p.Id == request.PageId, cancellationToken);

            if (page is null)
                throw new NotFoundException("Page not found");

            page.IsDeleted = true;
            page.DeletedBy = request.DeletedBy;
            page.DeletedTime = DateTime.UtcNow;

            foreach (var t in page.Translations)
            {
                t.IsDeleted = true;
                t.DeletedBy = request.DeletedBy;
                t.DeletedTime = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        public async Task<PaginatedResult<PageDto>> GetPagedList(GetFilteredPagesQuery request, CancellationToken cancellationToken)
        {
            var currentLanguage = L.CurrentLanguage;
            var query = from page in _context.Pages
                        where page.Type == request.Type
                        join user in _context.Users
                            on page.AuthorId equals user.Id into userGroup
                        from author in userGroup.DefaultIfEmpty()
                        select new
                        {
                            Page = page,
                            Author = author,
                            Translations = page.Translations
                                .Where(t => t.Culture == currentLanguage)
                                .ToList()
                        };

            if (!string.IsNullOrWhiteSpace(request.Title))
                query = query.Where(p => p.Translations.Any(t => t.Title!.Contains(request.Title)));

            if (!string.IsNullOrWhiteSpace(request.Introduction))
                query = query.Where(p => p.Translations.Any(t => t.Introduction!.Contains(request.Introduction)));

            if (!string.IsNullOrWhiteSpace(request.AuthorName))
                query = query.Where(p => p.Author.FullName != null && p.Author.FullName.Contains(request.AuthorName));

            if (request.FromDate.HasValue)
                query = query.Where(p => p.Page.CreationTime >= request.FromDate);

            if (request.ToDate.HasValue)
                query = query.Where(p => p.Page.CreationTime <= request.ToDate);

            if (request.Status.HasValue)
                query = query.Where(p => p.Page.Status == request.Status);

            // Sorting
            if (!string.IsNullOrEmpty(request.SortBy))
            {
                //var sortString = $"{request.SortBy} {request.SortDirection}";
                //query = query.OrderBy(sortString);
                query = query.OrderByDynamic(request.SortBy, request.SortDirection!);
            }

            var result = await query
                .Select(p => new PageDto
                {
                    Id = p.Page.Id,
                    Status = p.Page.Status,
                    Author = p.Author.FullName,
                    Created = p.Page.CreationTime,
                    Culture = p.Translations.FirstOrDefault()!.Culture,
                    Title = p.Translations.FirstOrDefault()!.Title ?? "",
                    Introduction = p.Translations.FirstOrDefault()!.Introduction,
                    Description = p.Translations.FirstOrDefault()!.Description
                }).ToListAsync(cancellationToken);

            return new PaginatedResult<PageDto>
            {
                Items = result,
                TotalCount = await query.CountAsync(cancellationToken),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PageForEditDto> GetPageForEdit(GetPageForEditMetaDataQuery request, CancellationToken cancellationToken)
        {
            var page = await _context.Pages
                       .Where(x => x.Id == request.pageId)
                       .Select(s => new PageForEditDto
                       {
                           Id = s.Id,
                           CoverImage = s.CoverImage,
                           IsActive = s.IsActive,
                           IsSystem = s.IsSystem,
                           Status = s.Status,
                           Type = s.Type,
                           Translations = s.Translations.Select(t => new PageMetaDataDto
                           {
                               Id = t.Id,
                               Title = t.Title ?? "",
                               Culture = t.Culture,
                               Introduction = t.Introduction,
                               Description = t.Description,
                               MetaDescription = t.MetaDescription,
                               MetaKeywords = t.MetaKeywords
                           }).ToList()
                       })
                       .FirstOrDefaultAsync();
            if (page == null)
            {
                throw new NotFoundException("Page not found");
            }

            return page;
        }


        public async Task<PageDto> GetPageInfo(GetPageInfoQuery request, CancellationToken cancellationToken)
        {
            var page = await _context.Pages
                  .Where(w => w.Id == request.PageId)
                 .Select(s => new
                 {
                     s.Id,
                     s.CoverImage,
                     s.IsActive,
                     s.IsSystem,
                     s.Status,
                     s.Type,
                     Translation = s.Translations
                                 .Where(t => t.Culture == request.Culture)
                                 .Select(t => new
                                 {
                                     t.Culture,
                                     t.Title,
                                     t.Description,
                                     t.MetaDescription,
                                     t.MetaKeywords,
                                     t.Html,
                                     t.Data,
                                     t.Styles,
                                     t.Scripts
                                 }).FirstOrDefault()
                 })
                   .FirstOrDefaultAsync();
            if (page == null)
            {
                throw new NotFoundException("Page not found");
            }
            return new PageDto()
            {
                Id = page.Id,
                Culture = page.Translation?.Culture ?? "",
                CoverImage = page.CoverImage,
                IsActive = page.IsActive,
                IsSystem = page.IsSystem,
                Status = page.Status,
                Type = page.Type,
                Title = page.Translation?.Title ?? "",
                Description = page.Translation?.Description,
                MetaDescription = page.Translation?.MetaDescription,
                MetaKeywords = page.Translation?.MetaKeywords,
                Html = page.Translation?.Html,
                Data = page.Translation?.Data,
                Styles = page.Translation?.Styles,
                Scripts = page.Translation?.Scripts,
            };
        }



        public async Task<Unit> UpdatePageContent(UpdatePageContentCommand request, CancellationToken cancellationToken)
        {
            var translation = await _context.PageTranslations
                .FirstOrDefaultAsync(pt => pt.PageId == request.PageId && pt.Culture == request.Culture, cancellationToken);

            if (translation is null)
            {
                translation = new PageTranslation
                {
                    Culture = request.Culture,
                    Data = request.Data,
                    Html = request.Html,
                    Styles = request.Styles,
                    Scripts = request.Scripts,
                };
            }
            else
            {
                translation.Data = request.Data;
                translation.Html = request.Html;
                translation.Styles = request.Styles;
                translation.Scripts = request.Scripts;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }





        #region Blocks
        public async Task<Guid> CreatePlugin(CreatePluginCommand request, CancellationToken cancellationToken)
        {
            // New page
            var block = new Plugin
            {
                Name = request.Name,
                Description = request.Description,
                CategoryName = request.CategoryName,
                Icon = request.Icon,
                Image = request.Image,
                Data = request.Data,
            };

            await _context.Plugins.AddAsync(block);

            await _context.SaveChangesAsync(cancellationToken);
            return block.Id;
        }

        public async Task<Unit> DeletePluginPage(DeletePluginCommand request, CancellationToken cancellationToken)
        {
            var page = await _context.Plugins
               .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

            if (page is null)
                throw new NotFoundException("Block not found");
            if (page.CreatedBy != _currentUserService.UserId)
            {
                throw new AccessDeniedException("AccessDenied!\nCan not delete this block!");
            }
            await _context.Plugins.Where(x => x.Id == request.Id).ExecuteDeleteAsync();


            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }

        public async Task<PaginatedResult<PluginDto>> GetPlugins(GetPluginQuery request, CancellationToken cancellationToken)
        {
            var query =
                from plugin in _context.Plugins
                join user in _context.Users
                    on plugin.CreatedBy equals user.Id.ToString() into users
                from author in users.DefaultIfEmpty()
                select new { plugin, author };

            if (!string.IsNullOrWhiteSpace(request.Filter))
            {
                query = query.Where(p =>
                    (p.author != null && p.author.FullName!.Contains(request.Filter)) ||
                    (p.plugin.Name != null && p.plugin.Name.Contains(request.Filter)) ||
                    (p.plugin.Description != null && p.plugin.Description.Contains(request.Filter))
                );
            }

            if (!string.IsNullOrEmpty(request.SortBy))
            {
                query = query.OrderByDynamic(request.SortBy, request.SortDirection!);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(p => new PluginDto
                {
                    Id = p.plugin.Id,
                    Name = p.plugin.Name,
                    Description = p.plugin.Description,
                    CategoryName = p.plugin.CategoryName,
                    Icon = p.plugin.Icon,
                    Image = p.plugin.Image,
                    Data = p.plugin.Data,
                    CanDelete = _currentUserService.UserId == p.plugin.CreatedBy
                })
                .ToListAsync(cancellationToken);

            return new PaginatedResult<PluginDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }


        #endregion
    }
}
