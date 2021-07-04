import './NavHeader.css';
import '../assets/fontawesome-free-5.14.0-web/css/fontawesome.min.css';
import '../assets/fontawesome-free-5.14.0-web/css/solid.min.css';
import '../assets/fontawesome-free-5.14.0-web/css/regular.min.css';

import DesktopNavMenu from './DesktopNavMenu';
import MobileNavMenu from './MobileNavMenu';
import React from 'react';

export const NavHeader = function (props) {

  function onSignOut(){
    if(props.signOut){
      props.signOut();
    }
  }

  return (
    <header id='app-header'>
      <h2 id="header-title">FOMO <span className="check icon"></span></h2>
      <nav id="header-nav-bar">
        <MobileNavMenu/>
        <DesktopNavMenu/>
      </nav>
      <div id="header-logout" role="button" onClick={onSignOut}>Sign out</div>
    </header>
  );
}

export default NavHeader;
