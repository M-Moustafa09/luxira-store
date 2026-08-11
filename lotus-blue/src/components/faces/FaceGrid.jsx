import {
  Sun,
  Flower2,
  Moon,
  BriefcaseBusiness,
  Eye,
  Leaf,
} from "lucide-react";

import FaceCard from "./FaceCard";

import dailyImg from "../../assets/faces/daily.png";
import softImg from "../../assets/faces/soft.png";
import eveningImg from "../../assets/faces/evening.png";
import workImg from "../../assets/faces/work.png";
import strongEyesImg from "../../assets/faces/strong-eyes.png";
import noMakeupImg from "../../assets/faces/no-makeup.png";

const looks = [
  {
    id: 2,
    title: "مكياج ناعم",
    image: softImg,
    icon: Flower2,
    iconColor: "#0B2E74",
  },
  {
    id: 1,
    title: "مكياج يومي",
    image: dailyImg,
    icon: Sun,
    iconColor: "#FF7F86",
  },
  {
    id: 4,
    title: "إطلالة للعمل",
    image: workImg,
    icon: BriefcaseBusiness,
    iconColor: "#0B2E74",
  },
  {
    id: 3,
    title: "مكياج سهرة",
    image: eveningImg,
    icon: Moon,
    iconColor: "#0B2E74",
  },
  {
    id: 6,
    title: "بدون مكياج",
    image: noMakeupImg,
    icon: Leaf,
    iconColor: "#0B2E74",
  },
  {
    id: 5,
    title: "عيون قوية",
    image: strongEyesImg,
    icon: Eye,
    iconColor: "#0B2E74",
  },
];

export default function FaceGrid() {
  return (
    <div className="grid grid-cols-2 gap-2">
      {looks.map((look) => (
        <FaceCard
          key={look.id}
          image={look.image}
          title={look.title}
          icon={look.icon}
          iconColor={look.iconColor}
        />
      ))}
    </div>
  );
}