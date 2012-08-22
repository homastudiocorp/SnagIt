using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.ContentManagement;
using WebStore.Models;
using Orchard.Core.Routable.Models;

namespace WebStore
{
    public class Migrations : DataMigrationImpl
    {

        IContentManager _contentManager;
        public Migrations(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public int Create()
        {
            // Creating table mag_WebStore_CategoryRecord
            SchemaBuilder.CreateTable("CategoryRecord", table => table
                .ContentPartRecord()
                .Column("CategoryGuid", DbType.Guid)
                .Column("MenuItemId", DbType.Int32)
                .Column<DateTime>("SynchronizationDate")
            );

            // Creating table mag_WebStore_ProductRecord
            SchemaBuilder.CreateTable("ProductRecord", table => table
                .ContentPartRecord()
                .Column("ProductGuid", DbType.Guid)
                .Column<DateTime>("SynchronizationDate")
            );
            
            ContentDefinitionManager.AlterPartDefinition("BasketPart", cfg => cfg.Attachable());
            ContentDefinitionManager.AlterPartDefinition("BasketDetailsPart", cfg => cfg.Attachable());
            ContentDefinitionManager.AlterPartDefinition("ProductPart", cfg => cfg.Attachable());
            ContentDefinitionManager.AlterPartDefinition("CategoryPart", cfg => cfg.Attachable());

            SchemaBuilder.CreateTable(
                "WebStoreConfigurationPartRecord",
                t =>
                    t.ContentPartRecord()
                    .Column<String>("MerchantId")
                    .Column<String>("CatalogId")
                    .Column<String>("ServicesPath")
            );

            ContentDefinitionManager.AlterTypeDefinition("Webstore Basket", type => type
                .WithPart("BasketPart")
                .WithSetting("Stereotype", "Widget")
            );

            ContentDefinitionManager.AlterTypeDefinition("Webstore Basket Details", type => type
                .WithPart("BasketDetailsPart")
                .WithSetting("Stereotype", "Widget")
            );

            ContentDefinitionManager.AlterTypeDefinition("Webstore Product", type => type
                .WithPart("MenuPart")
                .WithPart("BasketPart")
                .WithPart("ProductPart")
                .WithPart("CommonPart")
                .WithPart("RoutePart")
            );

            ContentDefinitionManager.AlterTypeDefinition("Webstore Category", type => type
                .WithPart("MenuPart")
                .WithPart("BasketPart")
                .WithPart("CategoryPart")
                .WithPart("CommonPart")
                .WithPart("RoutePart")

            );

            ContentDefinitionManager.AlterTypeDefinition("Basket Detail", type => type
                .WithPart("BasketDetailsPart")
                .WithPart("RoutePart"));
            ;

            RoutePart routePart = _contentManager.Create<RoutePart>("Basket Detail");
            routePart.Path = "BasketDetails";
            routePart.Slug = "BasketDetails";
            routePart.Title = "Basket Details";


            return 1;
        }
        
        public int UpdateFrom1()
        {
            SchemaBuilder.DropTable("ProductRecord");
            SchemaBuilder.DropTable("CategoryRecord");

            SchemaBuilder.AlterTable("WebStoreConfigurationPartRecord", t => t
                .AddColumn<String>("PaypalAccount")
                );
            
            ContentDefinitionManager.AlterPartDefinition("ProductPricePart", cfg => cfg.Attachable());
            ContentDefinitionManager.AlterPartDefinition("ProductStockPart", cfg => cfg.Attachable());
            ContentDefinitionManager.AlterPartDefinition("ProductAddToBasketPart", cfg => cfg.Attachable());            
            ContentDefinitionManager.AlterPartDefinition("OrderPart", cfg => cfg.Attachable());
            
            ContentDefinitionManager.AlterTypeDefinition("Order", type => type
                .WithPart("OrderPart")
                .WithPart("RoutePart"));


            ContentDefinitionManager.AlterTypeDefinition("Webstore Basket", type => type
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
            );

            RoutePart routePart = _contentManager.Create<RoutePart>("Order");
            routePart.Path = "WebStore/Order";
            routePart.Slug = "WebStore/Order";
            routePart.Title = "Order";

            SchemaBuilder.CreateTable("CategoryRecord", table => table
                .ContentPartRecord()
                .Column("CategoryGuid", DbType.Guid)
                .Column("Code", DbType.String)
                .Column<DateTime>("SynchronizationDate")
            );

            // Creating table mag_WebStore_ProductRecord
            SchemaBuilder.CreateTable("ProductRecord", table => table
                .ContentPartRecord()
                .Column("ProductGuid", DbType.Guid)
                .Column("SKU", DbType.String)                
                .Column<DateTime>("SynchronizationDate")
            );

            ContentDefinitionManager.AlterTypeDefinition("Webstore Product", type => type
                .RemovePart("MenuPart")
                .RemovePart("BasketPart")                
                .WithPart("ProductPricePart")
                //.WithPart("ProductStockPart")
                .WithPart("ProductAddToBasketPart")
                .WithPart("ContainablePart")
                .Creatable()
            );


            ContentDefinitionManager.AlterTypeDefinition("Webstore Category", type => type
                .RemovePart("MenuPart")
                .RemovePart("BasketPart")                     
                .WithPart("ContainerPart")
                .Creatable()
            );

            RoutePart bdroutePart = _contentManager.List<RoutePart>("Basket Detail").Single();

            bdroutePart.Path = "WebStore/BasketDetails";
            bdroutePart.Slug = "WebStore/BasketDetails";
            bdroutePart.Title = "Basket Details";
            return 2;
        }


    }
}
