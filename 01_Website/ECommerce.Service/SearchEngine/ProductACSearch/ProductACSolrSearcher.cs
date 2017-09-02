using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Entity.SearchEngine;
using ECommerce.Entity.SolrSearch;
using ECommerce.Utility.DataAccess.SearchEngine;
using ECommerce.Utility.DataAccess.SearchEngine.Solr;
using SolrNet;
using SolrNet.Commands.Parameters;

namespace ECommerce.Facade.SearchEngine
{
    class ProductACSolrSearcher : SolrSearcher<ProductACSearchRecord, ProductACSearchResult>
    {
        protected override string SolrCoreName
        {
            get { return "Core.ProductSearch.ReplicateData"; }
        }

        protected override QueryOptions BuildQueryOptions(SearchCondition condition)
        {
            QueryOptions queryOptions = base.BuildQueryOptions(condition);

            string keywordPrefix = condition.KeyWord;
            if (keywordPrefix == "*:*")
            {
                keywordPrefix = String.Empty;
            }
            keywordPrefix = keywordPrefix.ToLower();
            condition.KeyWord = "*:*";

            queryOptions.Rows = 0;
            queryOptions.Facet = new FacetParameters()
            {
                MinCount = 1,
                Limit = 10,
                Prefix = keywordPrefix,
                Sort = true,
                Queries = new ISolrFacetQuery[] {
                    new SolrFacetFieldQuery("p_producttitle_facet") 
                }
            };

            return queryOptions;
        }

        protected override ProductACSearchResult TransformSolrQueryResult(SolrQueryResults<ProductACSearchRecord> solrQueryResult, SearchCondition condition)
        {
            ProductACSearchResult result = new ProductACSearchResult();

            var productTitleFacet = solrQueryResult.FacetFields["p_producttitle_facet"];

            foreach (var facet in productTitleFacet)
            {
                result.Items.Add(new ProductACSearchResultItem()
                {
                    Keyword = facet.Key,
                    Quantity = facet.Value
                });
            }

            return result;
        }
    }
}
