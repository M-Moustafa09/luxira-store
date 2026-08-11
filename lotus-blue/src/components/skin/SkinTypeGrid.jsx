import {
  Droplets,
  Waves,
  Sparkles,
  Feather,
  CircleDot,
} from "lucide-react";

import SkinTypeCard from "./SkinTypeCard";

const skinTypes = [
  {
    id: "oily",
    title: "بشرة دهنية",
    description: "منتجات تتحكم باللمعان وتدوم طوال اليوم",
    icon: Droplets,
    iconColor: "#7CA6D8",
    iconBg: "#F1F7FD",
  },
  {
    id: "dry",
    title: "بشرة جافة",
    description: "مرطبات وتركيبات غنية لإشراقة ناعمة",
    icon: Droplets,
    iconColor: "#E6A6AA",
    iconBg: "#FFF5F5",
  },
  {
    id: "combination",
    title: "بشرة مختلطة",
    description: "توازن بين الترطيب والثبات",
    icon: Sparkles,
    iconColor: "#719B8A",
    iconBg: "#F3F8F5",
  },
  {
    id: "sensitive",
    title: "بشرة حساسة",
    description: "مكونات لطيفة وخالية من العطور",
    icon: Feather,
    iconColor: "#9B8DD0",
    iconBg: "#F8F5FD",
  },
  {
    id: "acne",
    title: "بشرة معرضة للحبوب",
    description: "تركيبات خفيفة لا تسد المسام",
    icon: CircleDot,
    iconColor: "#D69C73",
    iconBg: "#FFF8F1",
  },
];

export default function SkinTypeGrid() {
  return (
    <div className="flex flex-col gap-3">
      {skinTypes.map((skin) => (
        <SkinTypeCard
          key={skin.id}
          title={skin.title}
          description={skin.description}
          icon={skin.icon}
          iconColor={skin.iconColor}
          iconBg={skin.iconBg}
        />
      ))}
    </div>
  );
}