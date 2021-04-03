import './NavHeader.css';

import { NavLink } from 'react-router-dom';
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
        <div className="nav-header-item"><NavLink exact activeClassName="nav-header-item-selected" to="/">Portfolio</NavLink></div>
        <div className="nav-header-item"><NavLink activeClassName="nav-header-item-selected" to="/Leaderboard">Leaderboard</NavLink></div>
        <div className="nav-header-item"><NavLink activeClassName="nav-header-item-selected" to="/Members">Members</NavLink></div>
        <div className="nav-header-item"><NavLink activeClassName="nav-header-item-selected" to="/Faq">FAQ</NavLink></div>
        <div className="nav-header-item"><a className="github-link" href="https://github.com/GaryHuang501/Fomo" aria-label="github"> </a></div>
      </nav>
      <div id="header-logout" role="button" onClick={onSignOut}>Sign out</div>
    </header>
  );
}

export default NavHeader;
