import { fireEvent, screen, waitFor, within } from '@testing-library/react';

import MockAdapter from 'axios-mock-adapter';
import React from 'react';
import { StockSearchBar } from './StockSearchBar';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import { render } from '../../test-util';

let initialState = null;

beforeEach(() => {
    process.env = {
        REACT_APP_API_URL: "https://test.com",
        REACT_APP_STOCK_SEARCH_RESULTS_LIMIT: 5    
    };

    initialState = {
        portfolio: {
            ids: [1],
            selectedPortfolioId: 1,
            portfolios: {
                1: {
                    id: 1,
                    portfolioSymbols: []
                }
            }
        }
    };
});

afterEach(() => {
    process.env = {};
});

it("renders nothing when no keywords", async () => {

    act(() => {
        render(<StockSearchBar/>, { initialState });
    });

    fireEvent.change(screen.getByRole('searchbox'), { target: { value: '' } })

    expect(screen.queryByRole('option')).not.toBeInTheDocument();
});

it("renders searchbox with no results when no matches", async () => {

    const mock = new MockAdapter(axios);

    mock.onGet(`${process.env.REACT_APP_API_URL}/symbols`, { params : { keywords: 'IBM', limit: process.env.REACT_APP_STOCK_SEARCH_RESULTS_LIMIT }})
        .reply(200, []);

    act(() => {
        render(<StockSearchBar/>, { initialState });
    });

    fireEvent.change(screen.getByRole('searchbox'), { target: { value: 'IBM' } })

    await waitFor(() => expect(screen.getByText('No results found.')).toBeInTheDocument());
});

it("renders single search result", async () => {
    const mock = new MockAdapter(axios);
    const searchResults = [{symbolId: 1, ticker: 'IBM', fullName: 'International Business Machines'}];

    mock.onGet(`${process.env.REACT_APP_API_URL}/symbols`, { params : { keywords: 'IBM', limit: process.env.REACT_APP_STOCK_SEARCH_RESULTS_LIMIT }})
        .reply(200, searchResults);

    act(() => {
        render(<StockSearchBar/>, { initialState });
    });

    fireEvent.change(screen.getByRole('searchbox'), { target: { value: 'IBM' } })

    await waitFor(() => expect(within(screen.getByRole('option')).getByText('IBM')).toBeInTheDocument());
    const options = screen.getAllByRole('option');
    expect(options).toHaveLength(1);

    expect(within(options[0]).getByText('International Business Machines')).toBeInTheDocument();
});

it("renders multiples search result", async () => {
    const mock = new MockAdapter(axios);
    const searchResults = [
        {symbolId: 1, ticker: 'IBM', fullName: 'International Business Machines'},
        {symbolId: 2, ticker: 'IBMA', fullName: 'International Book Movement Associate'}
    ];
    mock.onGet(`${process.env.REACT_APP_API_URL}/symbols`, { params : { keywords: 'IBM', limit: process.env.REACT_APP_STOCK_SEARCH_RESULTS_LIMIT }})
        .reply(200, searchResults);

    act(() => {
        render(<StockSearchBar/>, { initialState });
    });

    fireEvent.change(screen.getByRole('searchbox'), { target: { value: 'IBM' } })

    await waitFor(() => expect(screen.getByText('IBM')).toBeInTheDocument());
    await waitFor(() => expect(screen.getByText('IBMA')).toBeInTheDocument());

    const options = screen.getAllByRole('option');
    expect(options).toHaveLength(2);

    expect(within(options[0]).getByText('International Business Machines')).toBeInTheDocument();
    expect(within(options[1]).getByText('International Book Movement Associate')).toBeInTheDocument();

});

it("closes search results when clicked outside of searchbox", async () => {
    const mock = new MockAdapter(axios);
    const searchResults = [{symbolId: 1, ticker: 'IBM', fullName: 'International Business Machines'}];

    mock.onGet(`${process.env.REACT_APP_API_URL}/symbols`, { params : { keywords: 'IBM', limit: process.env.REACT_APP_STOCK_SEARCH_RESULTS_LIMIT }})
        .reply(200, searchResults);

    const style = {height: '10px', width:'10px'};

    act(() => {
        render(<StockSearchBar/>, { initialState });
    });

    fireEvent.change(screen.getByRole('searchbox'), { target: { value: 'IBM' } })

    await waitFor(() => expect(within(screen.getByRole('option')).getByText('IBM')).toBeInTheDocument());

    fireEvent.mouseDown(document);

    await waitFor(() => expect(screen.queryByRole('option')).not.toBeInTheDocument());
});