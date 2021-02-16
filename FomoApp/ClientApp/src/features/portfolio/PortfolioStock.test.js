import { PortfolioStock }from './PortfolioStock';
import React from 'react';
import { act } from 'react-dom/test-utils';
import { render } from '../../test-util';
import { screen } from '@testing-library/react';

beforeEach(() => {
});

afterEach(() => {
});

it("renders with the portfolio symbol with no data", () => {
  const portfolioSymbol = { portfolioSymbolId: 1, symbolId: 1, ticker: 'abc'};

  act(() => {
    render(<table><tbody><PortfolioStock key={portfolioSymbol.symbolId} portfolioSymbol={portfolioSymbol}/></tbody></table>);
  });

  const columns = screen.queryAllByRole('cell');
  expect(columns[0].innerHTML).toBe(portfolioSymbol.ticker);
  expect(columns[1].innerHTML).toBe("Pending");
  expect(columns[2].innerHTML).toBe("--%");
  expect(columns[3].innerHTML).toBe("--");
  expect(columns[4].innerHTML).toBe("--%");
  expect(columns[5].innerHTML).toBe("0");
});

it("renders with the portfolio symbol with null data", () => {
  const portfolioSymbol = { portfolioSymbolId: 1, symbolId: 1, ticker: 'abc'};

  const initialState = {
      stocks: {
        singleQuoteData:
        {
          1: null
        }
      }
  };

  act(() => {
    render(<table><tbody><PortfolioStock key={portfolioSymbol.symbolId} portfolioSymbol={portfolioSymbol}/></tbody></table>, { initialState });
  });

  const columns = screen.queryAllByRole('cell');
  expect(columns[0].innerHTML).toBe(portfolioSymbol.ticker);
  expect(columns[1].innerHTML).toBe("Pending");
  expect(columns[2].innerHTML).toBe("--%");
  expect(columns[3].innerHTML).toBe("--");
  expect(columns[4].innerHTML).toBe("--%");
  expect(columns[5].innerHTML).toBe("0");
});


it("renders with the portfolio symbol with data", () => {
  const portfolioSymbol = { portfolioSymbolId: 1, symbolId: 1, ticker: 'abc'};
  const stockData = {
    symbolId: 1,
    ticker:  'SPY',
    price: 12.15,
    averagePrice: 12.50,
    changePercent: 12.50,
    votes: 5,
    return: 1
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
    render(<table><tbody><PortfolioStock key={portfolioSymbol.symbolId} portfolioSymbol={portfolioSymbol}/></tbody></table>, { initialState });
  });

  const columns = screen.queryAllByRole('cell');
  expect(columns[0].innerHTML).toBe(portfolioSymbol.ticker);
  expect(columns[1].innerHTML).toBe(stockData.price.toString());
  expect(columns[2].innerHTML).toBe(stockData.changePercent.toFixed(2) + "%");
  expect(columns[3].innerHTML).toBe(stockData.averagePrice.toString());
  expect(columns[4].innerHTML).toBe(stockData.return + "%");
  expect(columns[5].innerHTML).toBe(stockData.votes.toString());
});
