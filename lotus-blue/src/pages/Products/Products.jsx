import { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import { ArrowUpDown, Star } from "lucide-react";

import { TbLeaf } from "react-icons/tb";
import { AiOutlineTag } from "react-icons/ai";
import { GiPriceTag } from "react-icons/gi";
import { CiGrid41 } from "react-icons/ci";

import { useProductsStore } from "../../store/productsStore.js";
import { useWishlistStore } from "../../store/wishlistStore.js";
import { useCategoriesStore } from "../../store/categoriesStore.js";
import { useBrandsStore } from "../../store/brandsStore.js";

import ProductGridCard from "../../components/cards/ProductGridCard.jsx";
import Chip from "../../components/common/Chip.jsx";
import FilterBottomSheet from "../../components/search/FilterBottomSheet.jsx";
import OptionsBottomSheet from "../../components/products/OptionsBottomSheet.jsx";
import PriceBottomSheet from "../../components/products/PriceBottomSheet.jsx";

const sortMap = {
  bestSelling: "Relevance",
  priceAsc: "PriceAsc",
  priceDesc: "PriceDesc",
  rating: "Rating",
  newest: "Newest",
};

const ratingOptions = [
  { id: 5, label: "5 نجوم" },
  { id: 4, label: "4 نجوم فأعلى" },
  { id: 3, label: "3 نجوم فأعلى" },
  { id: 2, label: "نجمتان فأعلى" },
];

const skinTypeOptions = [
  { id: "Oily", label: "دهنية" },
  { id: "Dry", label: "جافة" },
  { id: "Combination", label: "مختلطة" },
  { id: "Sensitive", label: "حساسة" },
  { id: "Normal", label: "عادية" },
];

export default function Products({ wishlistOnly = false }) {
  const products = useProductsStore((s) => s.products);
  const fetchProducts = useProductsStore((s) => s.fetchProducts);
  const wishlistProducts = useWishlistStore((s) => s.products);
  const fetchWishlist = useWishlistStore((s) => s.fetchWishlist);
  const categories = useCategoriesStore((s) => s.categories);
  const fetchCategories = useCategoriesStore((s) => s.fetchCategories);
  const brands = useBrandsStore((s) => s.brands);
  const fetchBrands = useBrandsStore((s) => s.fetchBrands);

  const [searchParams] = useSearchParams();
  const [categoryId, setCategoryId] = useState(
    searchParams.get("category") ?? null,
  );
  const [brandId, setBrandId] = useState(null);
  const [minRating, setMinRating] = useState(null);
  const [skinType, setSkinType] = useState(null);
  const [priceRange, setPriceRange] = useState({ minPrice: null, maxPrice: null });
  const [sortValue, setSortValue] = useState("bestSelling");

  const [openSort, setOpenSort] = useState(false);
  const [openCategory, setOpenCategory] = useState(false);
  const [openBrand, setOpenBrand] = useState(false);
  const [openRating, setOpenRating] = useState(false);
  const [openSkinType, setOpenSkinType] = useState(false);
  const [openPrice, setOpenPrice] = useState(false);

  useEffect(() => {
    if (wishlistOnly) {
      fetchWishlist();
    } else {
      fetchProducts({
        categoryId: categoryId ?? undefined,
        brandId: brandId ?? undefined,
        minRating: minRating ?? undefined,
        skinType: skinType ?? undefined,
        minPrice: priceRange.minPrice ?? undefined,
        maxPrice: priceRange.maxPrice ?? undefined,
        sort: sortMap[sortValue],
        pageSize: 100,
      });
      fetchCategories();
      fetchBrands();
    }
  }, [
    fetchProducts,
    fetchWishlist,
    fetchCategories,
    fetchBrands,
    wishlistOnly,
    categoryId,
    brandId,
    minRating,
    skinType,
    priceRange,
    sortValue,
  ]);

  const list = wishlistOnly ? wishlistProducts : products;

  return (
    <div className="px-4 pt-2 pb-6">
      <div className="mb-2">
        <h1 className="text-xl md:text-4xl text-center font-bold text-[#00319D]">
          {wishlistOnly ? "المفضلة" : "جميع المنتجات"}
        </h1>

        <p className="text-sm md:text-2xl text-center text-gray-600">
          {list.length} منتج
        </p>
      </div>

      {!wishlistOnly && (
        <div className="mb-2 flex flex-wrap gap-2">
          <Chip
            icon={CiGrid41}
            active={openCategory}
            onClick={() => setOpenCategory(true)}
          >
            الفئة
          </Chip>

          <Chip
            icon={GiPriceTag}
            active={openPrice}
            onClick={() => setOpenPrice(true)}
          >
            السعر
          </Chip>

          <Chip
            icon={AiOutlineTag}
            active={openBrand}
            onClick={() => setOpenBrand(true)}
          >
            العلامة
          </Chip>

          <Chip
            icon={Star}
            active={openRating}
            onClick={() => setOpenRating(true)}
          >
            الدرجة
          </Chip>

          <Chip
            icon={TbLeaf}
            active={openSkinType}
            onClick={() => setOpenSkinType(true)}
          >
            البشرة
          </Chip>

          <Chip
            icon={ArrowUpDown}
            active={openSort}
            onClick={() => setOpenSort(true)}
          >
            ترتيب
          </Chip>
        </div>
      )}

      <div className="grid grid-cols-2 gap-2 lg:grid-cols-4">
        {list.map((product) => (
          <ProductGridCard key={product.id} product={product} />
        ))}
      </div>

      <FilterBottomSheet
        open={openSort}
        onClose={() => setOpenSort(false)}
        selected={sortValue}
        onSelect={(value) => {
          setSortValue(value);
          setOpenSort(false);
        }}
      />

      <OptionsBottomSheet
        open={openCategory}
        onClose={() => setOpenCategory(false)}
        title="اختر الفئة"
        allLabel="كل الفئات"
        options={categories.map((c) => ({ id: c.id, label: c.name }))}
        selected={categoryId}
        onSelect={(value) => {
          setCategoryId(value);
          setOpenCategory(false);
        }}
      />

      <OptionsBottomSheet
        open={openBrand}
        onClose={() => setOpenBrand(false)}
        title="اختر العلامة التجارية"
        allLabel="كل الماركات"
        options={brands.map((b) => ({ id: b.id, label: b.name }))}
        selected={brandId}
        onSelect={(value) => {
          setBrandId(value);
          setOpenBrand(false);
        }}
      />

      <OptionsBottomSheet
        open={openRating}
        onClose={() => setOpenRating(false)}
        title="التقييم"
        allLabel="كل التقييمات"
        options={ratingOptions}
        selected={minRating}
        onSelect={(value) => {
          setMinRating(value);
          setOpenRating(false);
        }}
      />

      <OptionsBottomSheet
        open={openSkinType}
        onClose={() => setOpenSkinType(false)}
        title="نوع البشرة"
        allLabel="كل أنواع البشرة"
        options={skinTypeOptions}
        selected={skinType}
        onSelect={(value) => {
          setSkinType(value);
          setOpenSkinType(false);
        }}
      />

      <PriceBottomSheet
        open={openPrice}
        onClose={() => setOpenPrice(false)}
        minPrice={priceRange.minPrice}
        maxPrice={priceRange.maxPrice}
        onApply={(value) => {
          setPriceRange(value);
          setOpenPrice(false);
        }}
      />
    </div>
  );
}
