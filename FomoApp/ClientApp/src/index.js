import './index.css';

import * as serviceWorker from './serviceWorker';

import { Route, BrowserRouter as Router, Routes } from 'react-router-dom';

import App from './App';
import { Provider } from 'react-redux';
import React from 'react';
import ReactDOM from "react-dom/client";
import RegistrationPage from './features/registration/RegistrationPage'
import { store } from './app/Store';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <Provider store={store}>
      <Router>
        <Routes>
          <Route path="/" element={<App/>}/>
          <Route path="portfolio/:urlUserId" element={<App/>}/>
          <Route path="Register" element={<RegistrationPage/>}/>
          <Route path="*" element={<App/>}/>
        </Routes>
      </Router>
    </Provider>
  </React.StrictMode>  
);

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
