import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { PortfolioContainer } from './containers/PortfolioContainer';
import { Community } from './components/Community';
import { LoginContainer } from './containers/LoginContainer';

export default class FomoApp extends Component {

  render() {
      return (
         <div id='rootWrapper'>
            <LoginContainer/>
            <Layout>
                <Route exact path='/' component={PortfolioContainer} />
                <Route path='/Community' component={Community} />
            </Layout>
        </div>
    );
  }
}
