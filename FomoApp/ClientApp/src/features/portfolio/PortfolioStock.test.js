import { screen, waitFor } from '@testing-library/react';

import { PortfolioStock } from './PortfolioStock';
import React from 'react';
import { act } from 'react-dom/test-utils';
import { render } from '../../test-util';

const symbolCol = 0;
const priceCol = 1;
const changeCol = 2;
const averagePriceCol = 3;
const returnCol = 4;
const votesCol = 5;

beforeEach(() => {
});

afterEach(() => {
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
    votes: 5,
    return: 1,
    myVote: 0
  };

  const initialState = {
    stocks: {
      singleQuoteData:
      {
        1: stockData
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
  await waitFor(() => expect(columns[votesCol].getElementsByClassName("portfolio-row-votes-value")[0].innerHTML.trim()).toEqual(stockData.votes.toString()));
});
