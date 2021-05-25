import * as reactRouter from 'react-router-dom';

import { screen, waitFor } from '@testing-library/react';

import MockAdapter from 'axios-mock-adapter';
import PortfolioPage from './PortfolioPage';
import React from 'react';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import { render } from '../../test-util';

let mockUseParams = {};

jest.mock('./PortfolioListener', () => () => (<div>Hello World</div>));
jest.mock('../chatbox/ChatBox', () => () => (<div>ChatBot!</div>));
jest.mock('react-router-dom', () => ({
    useParams: function(){
        return mockUseParams;
    }
}));

beforeEach(() => {
});

afterEach(() => {
    mockUseParams = {};
});

afterAll(() => {
    jest.restoreAllMocks();
});

it("renders portfolio for my user when visting page for myuser without id set in url", async () => {

    mockUseParams = {};
    const mock = new MockAdapter(axios);
    const portfolioId = 1;
    const userId = 100;

    const portfolio = {
        id: 1,
        portfolioSymbols: [ { id: 1, symbolId: 1, ticker: "VOO" }]
      };

    const loginState = {
        myUser: {
            id: userId,
            name: "myUser"
        }
    };

    const initialState = {
        login: loginState
    };
    
    mock.onGet(`${process.env.REACT_APP_API_URL}/accounts/${userId}`)
        .reply(200, { id: userId, name: "myUser" });

    mock.onGet(`${process.env.REACT_APP_API_URL}/portfolios?userId=${userId}`)
        .reply(200, [portfolioId]);

    mock.onGet(`${process.env.REACT_APP_API_URL}/portfolios/${portfolioId}`)
        .reply(200, portfolio);

    act(() => {
        render(<PortfolioPage />, { initialState: initialState });
    });

    await waitFor(() => expect(screen.getByRole("table")).toBeInTheDocument());
    await waitFor(() => expect(screen.getByText("VOO")).toBeInTheDocument());
    await waitFor(() => expect(screen.getByText("myUser's Portfolio")).toBeInTheDocument());
});

it("renders portfolio selected user portfolio when selected user in url", async () => {

    const mock = new MockAdapter(axios);
    const portfolioId = 1;
    const userId1 = 100;
    const userId2 = 200;
    mockUseParams = { urlUserId: userId2 };

    const portfolio = {
        id: 1,
        portfolioSymbols: [ { id: 1, symbolId: 1, ticker: "VOO" }]
    };

    const loginState = {
        myUser: {
            id: userId1,
            name: "myUser"
        }
    };

    const initialState = {
        login: loginState
    };
    
    mock.onGet(`${process.env.REACT_APP_API_URL}/accounts/${userId2}`)
        .reply(200, { id: userId2, name: "selectedUser" });

    mock.onGet(`${process.env.REACT_APP_API_URL}/portfolios?userId=${userId2}`)
        .reply(200, [portfolioId]);

    mock.onGet(`${process.env.REACT_APP_API_URL}/portfolios/${portfolioId}`)
        .reply(200, portfolio);

    act(() => {
        render(<PortfolioPage />, { initialState: initialState });
    });

    await waitFor(() => expect(screen.getByRole("table")).toBeInTheDocument());
    await waitFor(() => expect(screen.getByText("VOO")).toBeInTheDocument());
    await waitFor(() => expect(screen.getByText("selectedUser's Portfolio")).toBeInTheDocument());
});

it("renders stock search bar when current portfolio belongs to current user", async () => {

    mockUseParams = {};
    const userId = 100;

    const loginState = {
        myUser: {
            id: userId,
            name: "myUser"
        }
    };

    const initialState = {
        login: loginState
    };
    
    act(() => {
        render(<PortfolioPage />, { initialState: initialState });
    });

    await waitFor(() => expect(document.getElementById("stock-search-bar")).toBeInTheDocument());
});

it("should not render stock search bar when current portfolio does not belong to current user", async () => {

    const userId = 100;
    mockUseParams = { urlUserId: 999 };

    const loginState = {
        myUser: {
            id: userId,
            name: "myUser"
        }
    };

    const initialState = {
        login: loginState
    };
    
    act(() => {
        render(<PortfolioPage />, { initialState: initialState });
    });

    await waitFor(() => expect(document.getElementById("stock-search-bar")).not.toBeInTheDocument());
});