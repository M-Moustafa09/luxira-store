import { MdOutlineWaterDrop } from "react-icons/md";
import { BsDroplet } from "react-icons/bs";
import { PiLeafThin } from "react-icons/pi";
import { GoSun } from "react-icons/go";

export const needChips = [
  {
    id: "daily",
    label: "مكياج يومي",
    sub: "إطلالة طبيعية",
    icon: GoSun,
    color: "text-orange-500",
  },
  {
    id: "oily",
    label: "بشرة دهنية",
    sub: "منتجات متوازنة",
    icon: MdOutlineWaterDrop,
    color: "text-gray-900",
  },
  {
    id: "dry",
    label: "بشرة جافة",
    sub: "ترطيب عميق",
    icon: BsDroplet,
    color: "text-sky-500",
  },
  {
    id: "sensitive",
    label: "بشرة حساسة",
    sub: "مكونات لطيفة",
    icon: PiLeafThin,
    color: "text-green-800",
  },
];
