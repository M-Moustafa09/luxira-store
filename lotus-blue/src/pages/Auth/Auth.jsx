import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Mail, Lock, User, Phone } from "lucide-react";

import SectionCard from "../../components/checkout/SectionCard.jsx";
import CheckoutInput from "../../components/checkout/CheckoutInput.jsx";
import Button from "../../components/buttons/Button.jsx";
import { useAuthStore } from "../../store/authStore.js";

export default function Auth({ initialMode = "login" }) {
  const navigate = useNavigate();
  const login = useAuthStore((s) => s.login);
  const register = useAuthStore((s) => s.register);

  const [mode, setMode] = useState(initialMode);
  const [formData, setFormData] = useState({
    name: "",
    email: "",
    phone: "",
    password: "",
  });
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const isRegister = mode === "register";

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    setError("");
  };

  const toggleMode = () => {
    setMode(isRegister ? "login" : "register");
    setError("");
  };

  const handleSubmit = async () => {
    if (isRegister && !formData.name.trim()) {
      setError("من فضلك أدخلي الاسم الكامل");
      return;
    }

    if (!formData.email.trim()) {
      setError("من فضلك أدخلي البريد الإلكتروني");
      return;
    }

    if (!formData.password.trim()) {
      setError("من فضلك أدخلي كلمة المرور");
      return;
    }

    setError("");
    setIsSubmitting(true);

    try {
      if (isRegister) {
        await register(formData);
      } else {
        await login({ email: formData.email, password: formData.password });
      }

      navigate("/account");
    } catch (err) {
      setError(err.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <main
      dir="rtl"
      className="px-3 pb-10 pt-4"
    >
      <div className="mx-auto w-full max-w-sm">
        <h1 className="text-center text-lg font-medium text-[#00319D]">
          {isRegister ? "إنشاء حساب جديد" : "تسجيل الدخول"}
        </h1>

        <p className="mt-1 text-center text-[11px] text-[#00319D]/70">
          {isRegister
            ? "أنشئي حسابك للاحتفاظ بسلتك وطلباتك"
            : "سجّلي الدخول لمتابعة طلباتك وسلتك"}
        </p>

        <div className="mt-5 space-y-3">
          <SectionCard title={isRegister ? "بيانات الحساب" : "بيانات الدخول"}>
            <div className="space-y-2">
              {isRegister && (
                <CheckoutInput
                  icon={User}
                  placeholder="الاسم الكامل"
                  name="name"
                  value={formData.name}
                  onChange={handleChange}
                />
              )}

              <CheckoutInput
                icon={Mail}
                placeholder="البريد الإلكتروني"
                name="email"
                value={formData.email}
                onChange={handleChange}
              />

              {isRegister && (
                <CheckoutInput
                  icon={Phone}
                  placeholder="رقم الهاتف (اختياري)"
                  name="phone"
                  value={formData.phone}
                  onChange={handleChange}
                />
              )}

              <CheckoutInput
                icon={Lock}
                placeholder="كلمة المرور"
                name="password"
                type="password"
                value={formData.password}
                onChange={handleChange}
              />
            </div>
          </SectionCard>

          {error && (
            <p className="text-center text-[11px] text-red-500">{error}</p>
          )}

          <Button
            type="button"
            onClick={handleSubmit}
            disabled={isSubmitting}
            className="w-full !rounded-xl !py-2.5 !text-[13px] !font-normal"
          >
            {isSubmitting
              ? "جارٍ التنفيذ..."
              : isRegister
                ? "إنشاء الحساب"
                : "تسجيل الدخول"}
          </Button>

          <button
            type="button"
            onClick={toggleMode}
            className="w-full text-center text-[11px] text-[#00319D] underline underline-offset-2"
          >
            {isRegister
              ? "لديك حساب بالفعل؟ سجّلي الدخول"
              : "ليس لديك حساب؟ أنشئي واحداً"}
          </button>
        </div>
      </div>
    </main>
  );
}
