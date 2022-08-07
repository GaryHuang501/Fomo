import { NavLink } from 'react-router-dom';
import React from 'react';

export const NavHeaderItems = function () {

  return (
      <React.Fragment>
        <div className="nav-header-item"><NavLink className={({ isActive }) => (isActive ? "nav-header-item-selected" : "")} to="/">Portfolio</NavLink></div>
        <div className="nav-header-item"><NavLink className={({ isActive }) => (isActive ? "nav-header-item-selected" : "")}  to="/Leaderboard">Leaderboard</NavLink></div>
        <div className="nav-header-item"><NavLink className={({ isActive }) => (isActive ? "nav-header-item-selected" : "")}  to="/Friends">Friends</NavLink></div>
        <div className="nav-header-item"><NavLink className={({ isActive }) => (isActive ? "nav-header-item-selected" : "")}  to="/Faq">FAQ</NavLink></div>
        <div className="nav-header-item"><a className="github-link" href="https://github.com/GaryHuang501/Fomo" aria-label="github"> </a></div>
      </React.Fragment>
  );
}

export default NavHeaderItems;
