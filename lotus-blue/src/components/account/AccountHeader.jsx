import { Flower2 } from "lucide-react";
import DividerFlower from "../common/DividerFlower.jsx";

export default function AccountHeader() {
  return (
    <div className="mb-1 mt-2 flex flex-col items-center">
      <div className="mb-1 mt-2 flex flex-col items-center">
        <h1 className="text-[26px] font-semibold text-[#00319D]">الحساب</h1>

        <DividerFlower className=" h-5 w-[120px] text-[#F4A3B4]" />
      </div>
    </div>
  );
}
