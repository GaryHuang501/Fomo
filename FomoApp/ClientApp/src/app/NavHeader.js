import './NavHeader.css';
import '../assets/fontawesome-free-5.14.0-web/css/fontawesome.min.css';
import '../assets/fontawesome-free-5.14.0-web/css/solid.min.css';
import '../assets/fontawesome-free-5.14.0-web/css/regular.min.css';

import DesktopNavMenu from './DesktopNavMenu';
import FomoLogoBanner from '../app//FomoLogoBanner';
import MobileNavMenu from './MobileNavMenu';
import ProfileModal from './modal/ProfileModal';
import { React } from 'react';
import { showProfileModal } from '../app/modal/ModalSlice';
import { updateAccount } from '../features/login/LoginSlice';
import { useDispatch } from 'react-redux'

// Navigation header bar for changing pages on main app page
export const NavHeader = function (props) {

  const dispatch = useDispatch();
  
  function onSignOut() {
    if (props.signOut) {
      props.signOut();
    }
  }

  function openUserSettings() {
    dispatch(showProfileModal(true));
  }

  function closeUserSettings() {
    dispatch(showProfileModal(false));
  }

  function onUpdateUser(user){
    dispatch(updateAccount(user)); 
  }

  return (
    <FomoLogoBanner>
      <nav id="header-nav-bar">
        <MobileNavMenu />
        <DesktopNavMenu />
      </nav>
      <div id="header-logout" role="button" className="header-button" onClick={onSignOut}>Sign out</div>
      <i id="header-user-settings" className="fas fa-cog header-button" onClick={openUserSettings} role="button" />
      <ProfileModal onClose={closeUserSettings} onSubmit={onUpdateUser} ></ProfileModal>
    </FomoLogoBanner>
  );
}

export default NavHeader;
