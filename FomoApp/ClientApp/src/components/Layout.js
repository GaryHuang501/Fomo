import React, { Component } from 'react';
import { Route } from 'react-router';
import { NavHeader } from './NavHeader';
import { PortfolioOverviewBar } from './PortfolioOverviewBar';
import './Layout.css';


export class Layout extends Component {

    render() {
        return (
            <div id='layout-root'>
                <div id='layout-header-logo'>
                    <div id='header-logo'>FOMO</div>
                 </div>
                <NavHeader/>
                <div id='layout-overview-bar'>
                    <Route exact path='/' component={PortfolioOverviewBar} />
                </div>
                <div id='layout-leftsidebar'/>
                <main id='layout-content'>{this.props.children}</main>
                <div id='layout-rightsidebar'/>
                <footer id='layout-footer'/>
            </div>
        );
    }
}
