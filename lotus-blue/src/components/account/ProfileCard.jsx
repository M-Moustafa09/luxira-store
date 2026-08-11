import { useEffect, useState } from "react";
import { UserRound } from "lucide-react";

import { useAccountStore } from "../../store/accountStore.js";
import avatar from "../../assets/product-details/3.png";

export default function ProfileCard() {
  const profile = useAccountStore((s) => s.profile);
  const fetchProfile = useAccountStore((s) => s.fetchProfile);
  const updateProfile = useAccountStore((s) => s.updateProfile);

  const [isEditing, setIsEditing] = useState(false);
  const [formData, setFormData] = useState({ name: "", phone: "", email: "" });
  const [isSaving, setIsSaving] = useState(false);

  useEffect(() => {
    fetchProfile();
  }, [fetchProfile]);

  const startEditing = () => {
    setFormData({
      name: profile?.name ?? "",
      phone: profile?.phone ?? "",
      email: profile?.email ?? "",
    });
    setIsEditing(true);
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSave = async () => {
    setIsSaving(true);
    try {
      await updateProfile(formData);
      setIsEditing(false);
    } finally {
      setIsSaving(false);
    }
  };

  if (isEditing) {
    return (
      <section className="relative overflow-hidden rounded-md bg-[#FFF7F7] px-3 py-3">
        <div className="space-y-2">
          <input
            name="name"
            value={formData.name}
            onChange={handleChange}
            placeholder="الاسم الكامل"
            className="w-full rounded-md border border-[#ECECEC] bg-white px-3 py-2 text-[12px] text-[#00319D] outline-none"
          />

          <input
            name="phone"
            dir="ltr"
            value={formData.phone}
            onChange={handleChange}
            placeholder="رقم الهاتف"
            className="w-full rounded-md border border-[#ECECEC] bg-white px-3 py-2 text-[12px] text-[#00319D] outline-none"
          />

          <input
            name="email"
            dir="ltr"
            value={formData.email}
            onChange={handleChange}
            placeholder="البريد الإلكتروني"
            className="w-full rounded-md border border-[#ECECEC] bg-white px-3 py-2 text-[12px] text-[#00319D] outline-none"
          />
        </div>

        <div className="mt-3 flex gap-2">
          <button
            onClick={() => setIsEditing(false)}
            className="flex-1 rounded-full border border-[#00319D] py-1.5 text-[10px] text-[#00319D]"
          >
            إلغاء
          </button>

          <button
            onClick={handleSave}
            disabled={isSaving}
            className="flex-1 rounded-full bg-[#00319D] py-1.5 text-[10px] text-white"
          >
            {isSaving ? "جارٍ الحفظ..." : "حفظ"}
          </button>
        </div>
      </section>
    );
  }

  return (
    <section className="relative overflow-hidden rounded-md  bg-[#FFF7F7] px-3 py-2">
      {/* Lotus Background */}

      <div className="absolute -bottom-7 -left-6 opacity-15">
        <svg
          width="130"
          height="130"
          viewBox="0 0 120 120"
          fill="none"
          stroke="#F4A3B4"
          strokeWidth="1.5"
        >
          <path d="M60 14C52 28 52 44 60 56C68 44 68 28 60 14Z" />
          <path d="M60 56C75 41 90 39 102 50C90 66 75 68 60 56Z" />
          <path d="M60 56C45 41 30 39 18 50C30 66 45 68 60 56Z" />
          <path d="M60 56C52 70 52 88 60 104C68 88 68 70 60 56Z" />
        </svg>
      </div>

      <div className="flex items-center justify-between">
        {/* Avatar */}

        <div className="w-[30%] shrink-0">
          <img
            src={avatar}
            alt="avatar"
            className="h-[100px] w-[100px] rounded-full border-[3px] border-white object-contain shadow-sm"
          />
        </div>

        {/* Info */}

        <div className="flex flex-1 flex-col items-start mr-3">
          <h2 className="text-[20px] font-semibold text-[#00319D]">
            {profile?.name || "زائر"}
          </h2>

          <p
            dir="ltr"
            className="text-[13px] text-[#6D6D6D]"
          >
            {profile?.phone || "لم يتم إضافة رقم هاتف"}
          </p>

          <button
            onClick={startEditing}
            className="mt-1 flex h-7 items-center gap-1  rounded-full bg-[#00319D] px-3 text-[10px] text-white"
          >
            <UserRound size={12} />
            تعديل الملف الشخصي
          </button>
        </div>
      </div>
    </section>
  );
}
