import React from 'react';
import ReactDOM from 'react-dom';
import FomoApp from './FomoApp';
import registerServiceWorker from './registerServiceWorker';
import { Provider } from 'react-redux';
import { BrowserRouter } from 'react-router-dom';
import { createStore, applyMiddleware } from 'redux';
import { createLogger } from 'redux-logger';
import rootReducer from './reducers/rootReducer';
import thunkMiddleware from 'redux-thunk';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const rootElement = document.getElementById('root');
const loggerMiddleware = createLogger();
const store = createStore(
    rootReducer,
    applyMiddleware(
        thunkMiddleware,
        loggerMiddleware
    )
);

ReactDOM.render(
    <BrowserRouter basename={baseUrl}>
        <Provider store={store}>
            <FomoApp />
        </Provider>
    </BrowserRouter>

    , rootElement);

registerServiceWorker();
