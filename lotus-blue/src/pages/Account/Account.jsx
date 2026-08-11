import AccountHeader from "../../components/account/AccountHeader";
import AccountMenu from "../../components/account/AccountMenu.jsx";
import ProfileCard from "../../components/account/ProfileCard";

export default function Account() {
  return (
    <div className="px-3">
      <AccountHeader />

      <ProfileCard />

      <AccountMenu />
    </div>
  );
}
