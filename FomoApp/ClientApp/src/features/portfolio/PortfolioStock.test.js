import { fireEvent, screen, waitFor, within } from '@testing-library/react';

import MockAdapter from 'axios-mock-adapter';
import { Portfolio } from './Portfolio';
import { PortfolioStock } from './PortfolioStock';
import React from 'react';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import { render } from '../../test-util';

const symbolCol = 0;
const priceCol = 1;
const changeCol = 2;
const averagePriceCol = 3;
const returnCol = 4;
const votesCol = 5;

let mock;

beforeEach(() => {
  mock = new MockAdapter(axios);

  process.env = {
      REACT_APP_API_URL: "http://localhost"
  };
});

afterEach(() => {
  mock.restore();
  process.env = {};
});

it("renders with the portfolio symbol with no data", () => {
  const portfolioSymbol = { portfolioSymbolId: 1, symbolId: 1, ticker: 'abc' };

  act(() => {
    render(<table><tbody><PortfolioStock key={portfolioSymbol.symbolId} portfolioSymbol={portfolioSymbol} /></tbody></table>);
  });

  const columns = screen.queryAllByRole('cell');
  expect(columns[symbolCol].innerHTML).toBe(portfolioSymbol.ticker);
  expect(columns[priceCol].innerHTML).toBe("Pending");
  expect(columns[changeCol].innerHTML).toBe("--%");
  expect(columns[averagePriceCol].innerHTML).toBe("--");
  expect(columns[returnCol].innerHTML).toBe("--%");
  expect(columns[votesCol].getElementsByClassName("portfolio-row-votes-value")[0].innerHTML.trim()).toEqual("0");
});

it("renders with the portfolio symbol with null data", () => {
  const portfolioSymbol = { portfolioSymbolId: 1, symbolId: 1, ticker: 'abc' };

  const initialState = {
    stocks: {
      singleQuoteData:
      {
        1: null
      },
      votes: {
        1: null
      }
    }
  };

  act(() => {
    render(<table><tbody><PortfolioStock key={portfolioSymbol.symbolId} portfolioSymbol={portfolioSymbol} /></tbody></table>, { initialState });
  });

  const columns = screen.queryAllByRole('cell');
  expect(columns[symbolCol].innerHTML).toBe(portfolioSymbol.ticker);
  expect(columns[priceCol].innerHTML).toBe("Pending");
  expect(columns[changeCol].innerHTML).toBe("--%");
  expect(columns[averagePriceCol].innerHTML).toBe("--");
  expect(columns[returnCol].innerHTML).toBe("--%");
  expect(columns[votesCol].getElementsByClassName("portfolio-row-votes-value")[0].innerHTML.trim()).toEqual("0");
});


it("renders with the portfolio symbol with data", async () => {
  const portfolioSymbol = { portfolioSymbolId: 1, symbolId: 1, ticker: 'abc' };

  const stockData = {
    symbolId: 1,
    ticker: 'SPY',
    price: 12.15,
    averagePrice: 12.50,
    changePercent: 12.50,
    return: 1
  };

  const voteData = {
    symbolId: 1,
    count: 5,
    myVoteDirection: 0
  }

  const initialState = {
    stocks: {
      singleQuoteData:{
        1: stockData
      },
      votes:{
        1: voteData
      }
    }
  };

  act(() => {
    render(<table><tbody><PortfolioStock key={portfolioSymbol.symbolId} portfolioSymbol={portfolioSymbol} /></tbody></table>, { initialState });
  });

  const columns = screen.queryAllByRole('cell');
  expect(columns[symbolCol].innerHTML).toBe(portfolioSymbol.ticker);
  expect(columns[priceCol].innerHTML).toBe(stockData.price.toString());
  expect(columns[changeCol].innerHTML).toBe(stockData.changePercent.toFixed(2) + "%");
  expect(columns[averagePriceCol].innerHTML).toBe(stockData.averagePrice.toString());
  expect(columns[returnCol].innerHTML).toBe(stockData.return.toFixed(2) + "%");
  expect(columns[votesCol].getElementsByClassName("portfolio-row-votes-value")[0].innerHTML.trim()).toEqual(voteData.count.toString());
});


describe("Clicking portfolio stock options", () => {

  const initialPortfolio = {
    ids: [1],
    selectedPortfolioId: 1,
    portfolios: {
      1: {
        id: 1,
        portfolioSymbols: [
          { id: 1, symbolId: 1, ticker: "VOO" },
          { id: 2, symbolId: 2, ticker: "AMD" },
          { id: 3, symbolId: 3, ticker: "BA" }
        ]
      }
    }
  };

  const initialStockData = {
    singleQuoteData: {
      1: {}
    },
    votes: {}
  };

  const initialState = {
    portfolio: initialPortfolio,
    stocks: initialStockData
  };

  it("clicking up will move stock up", async () => {

    const url = `${process.env.REACT_APP_API_URL}/portfolios/1/portfolioSymbols/reorder`;
    mock.onPatch(url)
      .reply(200, {});

    const spy = jest.spyOn(axios, 'patch');

    act(() => {
      render(<Portfolio />, { initialState: initialState });
    });

    const upButtons = screen.getAllByTitle("Move Up");

    expect(upButtons.length).toEqual(3);

    const upButton = upButtons[1];

    fireEvent.click(upButton);

    await waitFor(() => expect(spy).toHaveBeenCalledWith(url, { portfolioSymbolIdToSortOrder: { 2: 0, 1: 1, 3: 2 }}));

    const stocks = document.getElementsByClassName('portfolio-stock');

    await waitFor(() => expect(stocks.length).toEqual(3));
    await waitFor(() => expect(within(stocks[0]).getByText("AMD")).toBeInTheDocument());
    expect(within(stocks[1]).getByText("VOO")).toBeInTheDocument();
    expect(within(stocks[2]).getByText("BA")).toBeInTheDocument();
  });

  it("clicking down will move stock down", async () => {

    const url = `${process.env.REACT_APP_API_URL}/portfolios/1/portfolioSymbols/reorder`;
    mock.onPatch(url)
      .reply(200, {});

    const spy = jest.spyOn(axios, 'patch');

    act(() => {
      render(<Portfolio />, { initialState: initialState });
    });

    const downButtons = screen.getAllByTitle("Move Down");

    expect(downButtons.length).toEqual(3);

    const downButton = downButtons[1];

    fireEvent.click(downButton);

    await waitFor(() => expect(spy).toHaveBeenCalledWith(url, { portfolioSymbolIdToSortOrder: { 1: 0, 3: 1, 2: 2 }}));

    const stocks = document.getElementsByClassName('portfolio-stock');

    await waitFor(() => expect(stocks.length).toEqual(3));
    await waitFor(() => expect(within(stocks[0]).getByText("VOO")).toBeInTheDocument());
    expect(within(stocks[1]).getByText("BA")).toBeInTheDocument();
    expect(within(stocks[2]).getByText("AMD")).toBeInTheDocument();
  });

  it("clicking delete button will remove stock", async () => {

    const amdPortfolioSymbolId = 2;
    const url = `${process.env.REACT_APP_API_URL}/portfolios/1/portfolioSymbols/${amdPortfolioSymbolId}`;

    mock.onDelete(url)
      .reply(200, {});

    const spy = jest.spyOn(axios, 'delete');

    act(() => {
      render(<Portfolio />, { initialState: initialState });
    });

    const deleteButtons = screen.getAllByTitle("Remove");

    expect(deleteButtons.length).toEqual(3);

    const deleteButton = deleteButtons[1];

    fireEvent.click(deleteButton);

    await waitFor(() => expect(spy).toHaveBeenCalledWith(url));

    const stocks = document.getElementsByClassName('portfolio-stock');

    await waitFor(() => expect(stocks.length).toEqual(2));
    await waitFor(() => expect(within(stocks[0]).getByText("VOO")).toBeInTheDocument());
    expect(within(stocks[1]).getByText("BA")).toBeInTheDocument();
  });
});