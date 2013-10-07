﻿using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.EntityBase;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Persistence.Caching;
using Merchello.Core.Persistence.Factories;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.Querying;
using Umbraco.Core.Persistence.UnitOfWork;

namespace Merchello.Core.Persistence.Repositories
{
    internal class ProductVariantRepository : MerchelloPetaPocoRepositoryBase<Guid, IProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(IDatabaseUnitOfWork work) 
            : base(work)
        { }

        public ProductVariantRepository(IDatabaseUnitOfWork work, IRepositoryCacheProvider cache) 
            : base(work, cache)
        { }

        #region Overrides MerchelloPetaPocoRepositoryBase

        protected override IProductVariant PerformGet(Guid id)
        {
            var sql = GetBaseQuery(false)
               .Where(GetBaseWhereClause(), new { Id = id });

            var dto = Database.Fetch<ProductDto, ProductVariantDto>(sql).FirstOrDefault();

            if (dto == null || dto.ProductVariantDto == null)
                return null;

            var factory = new ProductVariantFactory();

            var variant = factory.BuildEntity(dto.ProductVariantDto);

            variant.ResetDirtyProperties();

            return variant;
        }

        protected override IEnumerable<IProductVariant> PerformGetAll(params Guid[] ids)
        {
            if (ids.Any())
            {
                foreach (var id in ids)
                {
                    yield return Get(id);
                }
            }
            else
            {                
                var dtos = Database.Fetch<ProductDto, ProductVariantDto>(GetBaseQuery(false));
                foreach (var dto in dtos)
                {
                    yield return Get(dto.ProductVariantDto.Key);
                }
            }
        }

        protected override IEnumerable<IProductVariant> PerformGetByQuery(IQuery<IProductVariant> query)
        {
            var sqlClause = GetBaseQuery(false);
            var translator = new SqlTranslator<IProductVariant>(sqlClause, query);
            var sql = translator.Translate();

            var dtos = Database.Fetch<ProductDto, ProductVariantDto>(sql);

            return dtos.DistinctBy(x => x.Key).Select(dto => Get(dto.Key));

        }

        protected override Sql GetBaseQuery(bool isCount)
        {
            var sql = new Sql();
            sql.Select(isCount ? "COUNT(*)" : "*")
                .From<ProductDto>()
                .InnerJoin<ProductVariantDto>()
                .On<ProductDto, ProductVariantDto>(left => left.Key, right => right.ProductKey)
                .Where<ProductVariantDto>(x => x.Template == false);

            return sql;
        }

        protected override string GetBaseWhereClause()
        {
            return "merchProductVariant.pk = @Id";
        }

        protected override IEnumerable<string> GetDeleteClauses()
        {
            var list = new List<string>
            {
                "DELETE merchInventory WHERE productVariantKey = @Id",
                "DELETE merchProductVariant2ProductAttribute WHERE productVariantKey = @Id",
                "DELETE merchProductVariant WHERE pk = @Id"
            };

            return list;
        }
      

        protected override void PersistNewItem(IProductVariant entity)
        {
            if (!MandateProductVariantRules(entity)) return;

            ((Entity)entity).AddingEntity();

            var factory = new ProductVariantFactory();
            var dto = factory.BuildDto(entity);

            // insert the variant
            Database.Insert(dto);
            entity.Key = dto.Key;

            entity.ResetDirtyProperties();
        }

        protected override void PersistUpdatedItem(IProductVariant entity)
        {
            if (!MandateProductVariantRules(entity)) return;

            ((Entity)entity).UpdatingEntity();

            var factory = new ProductVariantFactory();
            var dto = factory.BuildDto(entity);

            // update the variant
            Database.Update(dto);

            entity.ResetDirtyProperties();
        }

        #endregion



        private bool MandateProductVariantRules(IProductVariant entity)
        {
            // TODO these checks can probably be moved somewhere else but are here at the moment to enforce the rules as the API develops
            Mandate.ParameterCondition(entity.ProductKey != Guid.Empty, "productKey must be set");

            if (!((ProductVariant)entity).Template)
                Mandate.ParameterCondition(entity.Attributes.Any(), "Product variant must have attributes");

            return true;
        }

        /// <summary>
        /// True/false indicating whether or not a sku is already exists in the database
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <returns></returns>
        public bool SkuExists(string sku)
        {
            var sql = new Sql();
            sql.Select("*")
               .From<ProductVariantDto>()
               .Where<ProductVariantDto>(x => x.Sku == sku);

            return Database.Fetch<ProductVariantDto>(sql).Any();

        }

    }
}