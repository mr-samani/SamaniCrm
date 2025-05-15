using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SamaniCrm.Application.Common.DTOs;
using SamaniCrm.Application.Common.Exceptions;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.DTOs;
using SamaniCrm.Application.Pages.Commands;
using SamaniCrm.Application.Pages.Queries;
using SamaniCrm.Domain.Entities;
using SamaniCrm.Infrastructure.Extensions;
using SamaniCrm.Infrastructure.Identity;

namespace SamaniCrm.Infrastructure.Services
{
    public class PageService : IPageService
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;



        public PageService(ApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> CreatePage(CreateOrEditPageMetaDataCommand request, CancellationToken cancellationToken)
        {
            Page? page;
            if (request.Id.HasValue)
            {
                page = await _context.Pages.FindAsync(request.Id.Value, cancellationToken);
                if (page == null)
                {
                    throw new NotFoundException("Page not found");
                }
            }
            else
            {
                page = new Page()
                {
                    AuthorId = Guid.Parse(_currentUserService.UserId!),
                    CoverImage = request.CoverImage,
                    IsActive = request.IsActive,
                    IsSystem = false,
                    Status = request.Status ?? PageStatusEnum.Draft,
                    Type = request.Type,
                };
                foreach (var item in request.Translations ?? [])
                {
                    page.Translations.Add(new PageTranslation()
                    {
                        Culture = item.Culture,
                        Introduction = item.Introduction,
                        Title = item.Title,
                        Description = item.Description,
                        MetaDescription = item.MetaDescription,
                        MetaKeywords = item.MetaKeywords,
                    });
                }
            }
            _context.Pages.Add(page);
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
            var currentLanguage = _currentUserService.lang ?? "en-US";
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
                query = query.Where(p => p.Translations.Any(t => t.Title.Contains(request.Title)));

            if (!string.IsNullOrWhiteSpace(request.Introduction))
                query = query.Where(p => p.Translations.Any(t => t.Introduction.Contains(request.Introduction)));

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
                    Title = p.Translations.FirstOrDefault().Title,
                    Introduction = p.Translations.FirstOrDefault().Introduction,
                    Description = p.Translations.FirstOrDefault().Description
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
                               Title = t.Title,
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
    }
}
