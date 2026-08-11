import { useNavigate } from "react-router-dom";
import DividerFlower from './../common/DividerFlower';

export default function FaceHeader() {

  return (
    <div className="relative mb-4 mt-2 flex flex-col items-center">

      <h1 className="text-[15px] font-semibold leading-none text-[#00319D]">
        التسوق حسب الإطلالة
      </h1>

      <DividerFlower className="mt-1 h-4 w-[125px] text-[#F3A3B0]" />

      <p className="mt-1 text-[10px] text-[#494949]">
        اختاري الإطلالة التي تناسبك
      </p>
    </div>
  );
}