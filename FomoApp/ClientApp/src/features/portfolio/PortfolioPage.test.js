import { screen, waitFor } from '@testing-library/react';

import MockAdapter from 'axios-mock-adapter';
import PortfolioPage from './PortfolioPage';
import React from 'react';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import { render } from '../../test-util';

jest.mock('./PortfolioListener', () => () => (<div>Hello World</div>));
jest.mock('../chatbox/ChatBox', () => () => (<div>ChatBot!</div>));

beforeEach(() => {
});

afterEach(() => {
});

afterAll(() => {
    jest.restoreAllMocks();
});

it("renders portfolio", async () => {

    const mock = new MockAdapter(axios);
    const portfolioId = 1;
    const portfolio = {
        id: 1,
        portfolioSymbols: [ { id: 1, symbolId: 1, ticker: "VOO" }]
      };

    mock.onGet(`${process.env.REACT_APP_API_URL}/portfolios`)
        .reply(200, [portfolioId]);

    mock.onGet(`${process.env.REACT_APP_API_URL}/portfolios/${portfolioId}`)
        .reply(200, portfolio);

    act(() => {
        render(<PortfolioPage />);
    });

    await waitFor(() => expect(screen.getByRole("table")).toBeInTheDocument());
    await waitFor(() => expect(screen.getByText("VOO")).toBeInTheDocument());
});
