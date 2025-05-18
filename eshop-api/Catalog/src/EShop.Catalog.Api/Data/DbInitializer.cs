using EShop.Catalog.Core.Models;
using EShop.Catalog.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EShop.Catalog.Api.Data;

public class DbInitializer
{
    public static async Task InitializeDbWIthTestData(CatalogDbContext catalogDbContext, string pictureUriPrefix)
    {
        await catalogDbContext.Database.MigrateAsync();

        if (!catalogDbContext.CatalogItems.Any())
        {
            var topSmartphonesBrand = new CatalogBrand("Smartphones For Everyone");
            var topCamerasBrand = new CatalogBrand("Best Cameras");
            var topLaptopBrand = new CatalogBrand("Top Laptops");
            var allForBikesBrand = new CatalogBrand("All For Bikes");

            var accessoriesType = new CatalogType("Accessories");
            var electronicsType = new CatalogType("Electronics");

            var catalogItems = new CatalogItem[]
                {
                    new CatalogItem("Bike Computer", electronicsType.Id, allForBikesBrand.Id, "Computer with GPS, odometer, and speedometer", 520, 12, @$"{pictureUriPrefix}/computer.jpg"),
                    new CatalogItem("Compact Bike Computer", electronicsType.Id, allForBikesBrand.Id, "Compact computer with GPS and mode selection", 390, 20, @$"{pictureUriPrefix}/computer_small.jpg"),
                    new CatalogItem("Gloves", accessoriesType.Id, allForBikesBrand.Id, "Bike globes", 30, 42, @$"{pictureUriPrefix}/gloves.jpg"),
                    new CatalogItem("Gloves, Black", accessoriesType.Id, allForBikesBrand.Id, "Gloves with color black", 40, 15, @$"{pictureUriPrefix}/black_gloves.jpg"),
                    new CatalogItem("MTB Gloves", accessoriesType.Id, allForBikesBrand.Id, "Gloves for Mountain Biking", 55, 21, @$"{pictureUriPrefix}/gloves_mtb.jpg"),
                    new CatalogItem("Helmet, Black", accessoriesType.Id, allForBikesBrand.Id, "Bike helmet, color black", 99, 30, @$"{pictureUriPrefix}/helmet_black.jpg"),
                    new CatalogItem("Helmet, Gray", accessoriesType.Id, allForBikesBrand.Id, "Bike helmet, color gray", 99, 40, @$"{pictureUriPrefix}/helmet_gray.jpg"),
                    new CatalogItem("Helmet, Red", accessoriesType.Id, allForBikesBrand.Id, "Bike helmet, color red", 99, 50, @$"{pictureUriPrefix}/helmet_red.jpg"),
                    new CatalogItem("Helmet, Yellow", accessoriesType.Id, allForBikesBrand.Id, "Bike helmet, color yellow", 99, 20, @$"{pictureUriPrefix}/helmet_yellow.jpg"),
                    new CatalogItem("DSLR Camera", electronicsType.Id, topCamerasBrand.Id, "Professional DSLR Camera", 3000, 5, @$"{pictureUriPrefix}/big_camera.jpg"),
                    new CatalogItem("Camera With Flash", electronicsType.Id, topCamerasBrand.Id, "Camera. Flash included", 3500, 3, @$"{pictureUriPrefix}/camera_with_flash.jpg"),
                    new CatalogItem("Compact Camera", electronicsType.Id, topCamerasBrand.Id, "Compact camera, color white", 742, 19, @$"{pictureUriPrefix}/compact_white_camera.jpg"),
                    new CatalogItem("Mirrorless Camera", electronicsType.Id, topCamerasBrand.Id, "Mirrorless camera, metal body", 1299, 4, @$"{pictureUriPrefix}/steel_camera.jpg"),
                    new CatalogItem("High Perfomance Laptop", electronicsType.Id, topLaptopBrand.Id, "Laptop, color black", 2400, 5, @$"{pictureUriPrefix}/black_laptop.jpg"),
                    new CatalogItem("Office Laptop", electronicsType.Id, topLaptopBrand.Id, "Office laptop, color silver", 1290, 20, @$"{pictureUriPrefix}/silver_laptop.jpg"),
                    new CatalogItem("Gaming Laptop", electronicsType.Id, topLaptopBrand.Id, "Laptop, for gamers", 1999, 40, @$"{pictureUriPrefix}/gaming_laptop.jpg"),
                    new CatalogItem("White Laptop", electronicsType.Id, topLaptopBrand.Id, "Laptop, white color", 150, 22, @$"{pictureUriPrefix}/white_laptop.jpg"),
                    new CatalogItem("Smartphone", electronicsType.Id, topSmartphonesBrand.Id, "Smartphone, color black", 800, 20, @$"{pictureUriPrefix}/smartphone.jpg"),
                    new CatalogItem("Silver Smartphone Case", accessoriesType.Id, topSmartphonesBrand.Id, "Case, color silver", 99, 50, @$"{pictureUriPrefix}/silver_smartphone_case.jpg"),
                    new CatalogItem("White Smartphone Case", accessoriesType.Id, topSmartphonesBrand.Id, "Case, color white", 79, 10, @$"{pictureUriPrefix}/white_smartphone_case.jpg")
                };

            catalogDbContext.CatalogBrands.AddRange(topSmartphonesBrand, topCamerasBrand, topLaptopBrand, allForBikesBrand);
            catalogDbContext.CatalogTypes.AddRange(accessoriesType,
                electronicsType);
            catalogDbContext.CatalogItems.AddRange(catalogItems);

            await catalogDbContext.SaveChangesAsync();
        }
    }
}
