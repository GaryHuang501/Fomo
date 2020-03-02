import React, { Component } from 'react';
import { NavLink } from 'react-router-dom';
import './NavHeader.css';

export class NavHeader extends Component {
    render() {
        return (
            <div id='layout-nav-header'>
                <div id="nav-header-items">
                    <NavLink className="nav-header-item" activeClassName="nav-header-item-selected" exact to={"/"}>Portfolio</NavLink>
                    <NavLink className="nav-header-item" activeClassName="nav-header-item-selected" to={"/Symbols"}>Symbols</NavLink>
                    <NavLink className="nav-header-item" activeClassName="nav-header-item-selected" to={"/Community"}>Community</NavLink>
                </div>
            </div>
        );
    }
}
