using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamaniCrm.Application.ProductManagerManager.Dtos;
using SamaniCrm.Domain.Entities.PageBuilderEntities;
using SamaniCrm.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.Persistence;

public class SeedDataSources
{
    public static async Task TrySeedAsync(ApplicationDbContext dbContext)
    {
        Console.WriteLine("Try seed Datasources");
        var existDatasources = await dbContext.DataSources.ToListAsync();
        if (!existDatasources.Any())
        {
            List<DataSource> ds = [
            new DataSource()
        {
            Title = "Products",
            DataSourceType = DataSourceTypeEnum.Products,
            Fields = [
                new DataSourceField(){
                    NameSpace="Id",
                    Title="Id",
                    Type=DataFieldTypeEnum.String,
                } ,
                new DataSourceField(){
                    NameSpace="CategoryId",
                    Title="Category id",
                    Type=DataFieldTypeEnum.String,
                },
                new DataSourceField(){
                    NameSpace="Title",
                    Title="Title",
                    Type=DataFieldTypeEnum.String,
                },
                new DataSourceField(){
                    NameSpace="Description",
                    Title="Description",
                    Type=DataFieldTypeEnum.String,
                },
                new DataSourceField(){
                    NameSpace="Content",
                    Title="Content",
                    Type=DataFieldTypeEnum.String,
                },
                new DataSourceField(){
                    NameSpace="Category title",
                    Title="CategoryTitle",
                    Type=DataFieldTypeEnum.String,
                },
                new DataSourceField(){
                    NameSpace="ProductTypeTitle",
                    Title="Product type title",
                    Type=DataFieldTypeEnum.String,
                },
                new DataSourceField(){
                    NameSpace="Tags",
                    Title="Tags",
                    Type=DataFieldTypeEnum.String,
                },
                new DataSourceField(){
                    NameSpace="SKU",
                    Title="SKU",
                    Type=DataFieldTypeEnum.String,
                },
                new DataSourceField(){
                    NameSpace="Slug",
                    Title="Slug",
                    Type=DataFieldTypeEnum.String,
                },
                new DataSourceField(){
                    NameSpace="CreationTime",
                    Title="Creation time",
                    Type=DataFieldTypeEnum.DateTime,
                },
                new DataSourceField(){
                    NameSpace="Images",
                    Title="Images",
                    Type=DataFieldTypeEnum.ArrayString,
                }
                ]
        },
           new DataSource()
        {
            Title = "ProductCategories",
            DataSourceType = DataSourceTypeEnum.ProductCategories,
            Fields = [
                new DataSourceField(){
                    NameSpace="Id",
                    Title="Id",
                    Type=DataFieldTypeEnum.String,
                } , new DataSourceField(){
                    NameSpace="Title",
                    Title="Title",
                    Type=DataFieldTypeEnum.String,
                },
                new DataSourceField(){
                    NameSpace="Description",
                    Title="Description",
                    Type=DataFieldTypeEnum.String,
                },
                new DataSourceField(){
                    NameSpace="Slug",
                    Title="Slug",
                    Type=DataFieldTypeEnum.String,
                },
                new DataSourceField(){
                    NameSpace="Images",
                    Title="Images",
                    Type=DataFieldTypeEnum.String,
                }]
            }];

            await dbContext.DataSources.AddRangeAsync(ds);
            await dbContext.SaveChangesAsync();
        }
    }
}

