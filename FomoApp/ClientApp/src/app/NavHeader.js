import React from 'react';
import './NavHeader.css';
import { NavLink } from 'react-router-dom'

export const NavHeader = function() {
  return (
    <header id='app-header'>
      <h2 id="header-title">FOMO <span className="check icon"></span></h2>    
      <nav id="header-nav-bar">
          <div className="nav-header-item"><NavLink exact activeClassName="nav-header-item-selected" to="/">Portfolio</NavLink></div>
          <div className="nav-header-item"><NavLink activeClassName="nav-header-item-selected" to="/Leaderboard">Leaderboard</NavLink></div>
          <div className="nav-header-item"><NavLink activeClassName="nav-header-item-selected" to="/Members">Members</NavLink></div>
          <div className="nav-header-item"><NavLink activeClassName="nav-header-item-selected" to="/About">About</NavLink></div>
          <div className="nav-header-item"><a className="github-link" href="https://github.com/GaryHuang501/Fomo" aria-label="github"> </a></div>
      </nav>
    </header>
  );
}

export default NavHeader;
