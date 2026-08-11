import { useEffect, useState } from "react";
import { Search as SearchIcon, History } from "lucide-react";

import SearchBar from "../../components/search/SearchBar";
import SearchSectionTitle from "../../components/search/SearchSectionTitle";
import SearchChip from "../../components/search/SearchChip";
import SearchHistoryItem from "../../components/search/SearchHistoryItem";
import SearchResultsHeader from "../../components/search/SearchResultsHeader";
import FilterBottomSheet from "../../components/search/FilterBottomSheet";
import ProductGridCard from "../../components/cards/ProductGridCard";

import { trendingSearches } from '../../data/search';

import { useSearchStore } from "../../store/searchStore";
import { useProductsStore } from "../../store/productsStore.js";

const sortMap = {
  bestSelling: "Relevance",
  priceAsc: "PriceAsc",
  priceDesc: "PriceDesc",
  rating: "Rating",
  newest: "Newest",
};

export default function Search() {
  const [openFilter, setOpenFilter] = useState(false);
  const [selectedSort, setSelectedSort] = useState("bestSelling");

  const query = useSearchStore((s) => s.query);
  const setQuery = useSearchStore((s) => s.setQuery);

  const recentSearches = useSearchStore((s) => s.recentSearches);
  const addRecentSearch = useSearchStore((s) => s.addRecentSearch);
  const removeRecentSearch = useSearchStore((s) => s.removeRecentSearch);

  const filteredProducts = useProductsStore((s) => s.products);
  const fetchProducts = useProductsStore((s) => s.fetchProducts);

  useEffect(() => {
    const timeout = setTimeout(() => {
      fetchProducts({ search: query.trim(), sort: sortMap[selectedSort], pageSize: 50 });
    }, 300);

    return () => clearTimeout(timeout);
  }, [query, selectedSort, fetchProducts]);

  const isSearching = query.trim().length > 0;

  return (
    <main className="min-h-screen">
      <div className="pt-2">
        {/* Search */}
        <section className="px-4">
          <SearchBar
            value={query}
            onChange={(e) => setQuery(e.target.value)}
            onSearch={() => {
              if (query.trim()) {
                addRecentSearch(query);
              }
            }}
          />
        </section>

        {/* Trending */}
        {!isSearching && (
          <section className="mt-3 px-4">
            <SearchSectionTitle title="عمليات بحث شائعة" icon={SearchIcon} />

            <div className="mt-1 flex gap-2 overflow-x-auto no-scrollbar">
              {trendingSearches.slice(0, 5).map((item) => (
                <SearchChip
                  key={item}
                  label={item}
                  onClick={() => {
                    setQuery(item);
                    addRecentSearch(item);
                  }}
                />
              ))}
            </div>
          </section>
        )}

        {/* Recent */}
        {!isSearching && recentSearches.length > 0 && (
          <section className="mt-5 px-4">
            <SearchSectionTitle title="آخر عمليات البحث" icon={History} />

            <div dir="ltr" className="mt-2 overflow-hidden rounded-md bg-white">
              {recentSearches.map((item, index) => (
                <SearchHistoryItem
                  key={item}
                  label={item}
                  onClick={() => {
                    setQuery(item);
                    addRecentSearch(item);
                  }}
                  onRemove={() => removeRecentSearch(item)}
                  isFirst={index === 0}
                  isLast={index === recentSearches.length - 1}
                />
              ))}
            </div>
          </section>
        )}

        {/* Results */}
        <section className="mt-3 bg-[#F0F0F0]/30 px-4">
          <SearchResultsHeader
            count={filteredProducts.length}
            onFilter={() => setOpenFilter(true)}
          />

          {filteredProducts.length > 0 ? (
            <div className="mt-1 grid grid-cols-2 gap-x-3 gap-y-4">
              {filteredProducts.map((product) => (
                <ProductGridCard key={product.id} product={product} />
              ))}
            </div>
          ) : (
            <div className="mt-14 text-center">
              <p className="text-[15px] text-[#00319D]">لا توجد نتائج</p>

              <p className="mt-2 text-[12px] text-gray-600">
                جرّب البحث بكلمة أخرى
              </p>
            </div>
          )}
        </section>
      </div>

      <FilterBottomSheet
        open={openFilter}
        onClose={() => setOpenFilter(false)}
        selected={selectedSort}
        onSelect={(value) => {
          setSelectedSort(value);
          setOpenFilter(false);
        }}
      />
    </main>
  );
}
