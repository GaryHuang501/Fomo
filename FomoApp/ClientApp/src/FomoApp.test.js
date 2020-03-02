import React from 'react';
import ReactDOM from 'react-dom';
import FomoApp from './FomoApp';
import { BrowserRouter } from 'react-router-dom'; 
import { Provider } from 'react-redux';
import { createStore, applyMiddleware } from 'redux';
import { createLogger } from 'redux-logger';
import rootReducer from './reducers/rootReducer';
import thunkMiddleware from 'redux-thunk';
import { fetchMock, MATCHED, UNMATCHED, Response } from 'fetch-mock';

const loggerMiddleware = createLogger();
const store = createStore(
    rootReducer,
    applyMiddleware(
        thunkMiddleware,
        loggerMiddleware
    )
);

beforeEach(function () {
    fetchMock.get('*', 200);
});

afterEach(function () {
    fetchMock.reset();
});

it('renders without crashing', () => {

    const div = document.createElement('div');
    ReactDOM.render(
        <BrowserRouter>
            <Provider store={store}>
                <FomoApp />
            </Provider>
        </BrowserRouter>
        , div);
});
