import './index.css';

import * as serviceWorker from './serviceWorker';

import { Redirect, Route, BrowserRouter as Router, Switch } from 'react-router-dom';

import App from './App';
import { Provider } from 'react-redux';
import React from 'react';
import ReactDOM from 'react-dom';
import RegistrationPage from './features/registration/RegistrationPage'
import { store } from './app/Store';

ReactDOM.render(
  <React.StrictMode>
    <Provider store={store}>
      <Router>
        <Switch>
          <Route exact path="/" component={App}/>
          <Route exact path="/Register" component={RegistrationPage}/>
          <Redirect to='/' />
        </Switch>
      </Router>
    </Provider>
  </React.StrictMode>,
  document.getElementById('root')
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
