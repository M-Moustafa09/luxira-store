import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { ChevronRight, User, Phone, MapPin, Map, Home, Tag, Pencil, Trash2 } from "lucide-react";

import { useAddressesStore } from "../../store/addressesStore.js";
import { SAUDI_CITIES } from "../../data/saudiCities.js";
import SectionCard from "../../components/checkout/SectionCard.jsx";
import CheckoutInput from "../../components/checkout/CheckoutInput.jsx";
import Button from "../../components/buttons/Button.jsx";

const emptyForm = {
  label: "",
  fullName: "",
  phone: "",
  city: "",
  region: "",
  addressDetails: "",
  isDefault: false,
};

export default function Addresses() {
  const addresses = useAddressesStore((s) => s.addresses);
  const fetchAddresses = useAddressesStore((s) => s.fetchAddresses);
  const createAddress = useAddressesStore((s) => s.createAddress);
  const updateAddress = useAddressesStore((s) => s.updateAddress);
  const deleteAddress = useAddressesStore((s) => s.deleteAddress);

  const [editingId, setEditingId] = useState(null);
  const [showForm, setShowForm] = useState(false);
  const [formData, setFormData] = useState(emptyForm);
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    fetchAddresses();
  }, [fetchAddresses]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const openCreateForm = () => {
    setEditingId(null);
    setFormData(emptyForm);
    setShowForm(true);
  };

  const openEditForm = (address) => {
    setEditingId(address.id);
    setFormData({
      label: address.label ?? "",
      fullName: address.fullName,
      phone: address.phone,
      city: address.city,
      region: address.region,
      addressDetails: address.addressDetails,
      isDefault: address.isDefault,
    });
    setShowForm(true);
  };

  const handleSave = async () => {
    setIsSubmitting(true);
    try {
      if (editingId) {
        await updateAddress(editingId, formData);
      } else {
        await createAddress(formData);
      }
      setShowForm(false);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="px-4 pb-6 pt-2">
      <div className="mb-2 flex items-center gap-2">
        <Link to="/account">
          <ChevronRight size={20} className="text-[#00319D]" />
        </Link>

        <h1 className="text-[18px] font-semibold text-[#00319D]">عناويني</h1>
      </div>

      <div className="space-y-2">
        {addresses.map((address) => (
          <div
            key={address.id}
            className="rounded-md border border-[#ECECEC] bg-white p-3"
          >
            <div className="flex items-start justify-between">
              <div>
                {address.label && (
                  <span className="mb-1 inline-block rounded-full bg-[#FFF4F5] px-2 py-0.5 text-[9px] text-[#00319D]">
                    {address.label}
                  </span>
                )}
                {address.isDefault && (
                  <span className="mb-1 mr-1 inline-block rounded-full bg-[#00319D] px-2 py-0.5 text-[9px] text-white">
                    افتراضي
                  </span>
                )}
                <p className="text-[13px] font-medium text-[#00319D]">
                  {address.fullName}
                </p>
                <p
                  dir="ltr"
                  className="text-[11px] text-[#6D6D6D]"
                >
                  {address.phone}
                </p>
                <p className="mt-1 text-[11px] text-[#6D6D6D]">
                  {address.city} - {address.region} - {address.addressDetails}
                </p>
              </div>

              <div className="flex shrink-0 items-center gap-2">
                <button onClick={() => openEditForm(address)}>
                  <Pencil size={15} className="text-[#00319D]" />
                </button>

                <button onClick={() => deleteAddress(address.id)}>
                  <Trash2 size={15} className="text-red-500" />
                </button>
              </div>
            </div>
          </div>
        ))}

        {addresses.length === 0 && !showForm && (
          <p className="mt-6 text-center text-[12px] text-[#8F97AE]">
            لا توجد عناوين محفوظة
          </p>
        )}
      </div>

      {showForm ? (
        <div className="mt-3">
          <SectionCard title={editingId ? "تعديل العنوان" : "عنوان جديد"}>
            <div className="space-y-2">
              <CheckoutInput
                icon={Tag}
                placeholder="اسم العنوان (مثال: المنزل)"
                name="label"
                value={formData.label}
                onChange={handleChange}
              />

              <CheckoutInput
                icon={User}
                placeholder="الاسم الكامل"
                name="fullName"
                value={formData.fullName}
                onChange={handleChange}
              />

              <CheckoutInput
                icon={Phone}
                placeholder="رقم الهاتف"
                name="phone"
                value={formData.phone}
                onChange={handleChange}
              />

              <div className="grid grid-cols-2 gap-3">
                <CheckoutInput
                  select
                  icon={MapPin}
                  label="المدينة"
                  name="city"
                  value={formData.city}
                  onChange={handleChange}
                  options={SAUDI_CITIES}
                />

                <CheckoutInput
                  icon={Map}
                  placeholder="الحي / المنطقة"
                  name="region"
                  value={formData.region}
                  onChange={handleChange}
                />
              </div>

              <CheckoutInput
                textarea
                icon={Home}
                label="العنوان"
                placeholder="ادخلي عنوانك بالتفصيل"
                name="addressDetails"
                value={formData.addressDetails}
                onChange={handleChange}
              />

              <label className="flex items-center gap-2 text-[11px] text-[#00319D]">
                <input
                  type="checkbox"
                  checked={formData.isDefault}
                  onChange={(e) =>
                    setFormData((prev) => ({ ...prev, isDefault: e.target.checked }))
                  }
                />
                اجعله العنوان الافتراضي
              </label>
            </div>
          </SectionCard>

          <div className="mt-3 flex gap-2">
            <Button
              variant="outline"
              className="flex-1 !rounded-xl !py-2 !text-[12px]"
              onClick={() => setShowForm(false)}
            >
              إلغاء
            </Button>

            <Button
              onClick={handleSave}
              disabled={isSubmitting}
              className="flex-1 !rounded-xl !py-2 !text-[12px]"
            >
              {isSubmitting ? "جارٍ الحفظ..." : "حفظ"}
            </Button>
          </div>
        </div>
      ) : (
        <Button
          onClick={openCreateForm}
          className="mt-3 w-full !rounded-xl !py-2 !text-[12px]"
        >
          إضافة عنوان جديد
        </Button>
      )}
    </div>
  );
}
