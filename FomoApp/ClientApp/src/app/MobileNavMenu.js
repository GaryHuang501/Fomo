import React, { useState } from 'react';

import { NavHeaderItems } from './NavHeaderItems';

export const MobileNavMenu = function (props) {

    const [showMobileMenu, setShowMobileMenu] = useState(false);

    function onClickShowMobileMenu() {
        setShowMobileMenu(true);
    }

    function onClickCloseMobileMenu() {
        setShowMobileMenu(false);
    }

    return (
        <div id="mobile-header-nav-menu">
            <i className="fas fa-bars" id="mobile-nav-menu-button" role='button' onClick={onClickShowMobileMenu}></i>
            {showMobileMenu ?
                <div id='mobile-header-nav-popup'>
                    <div id='mobile-header-nav-popup-close' onClick={onClickCloseMobileMenu}>X</div>
                    <NavHeaderItems></NavHeaderItems>
                </div> : null
            }
        </div>
    );
}

export default MobileNavMenu;
