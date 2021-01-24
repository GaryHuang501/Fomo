import { Portfolio }from './Portfolio';
import React from 'react';
import { act } from 'react-dom/test-utils';
import { render } from '../../test-util';
import { screen } from '@testing-library/react';

beforeEach(() => {
});

afterEach(() => {
});

it("renders table headers only when no portfolios stocks added", () => {

  act(() => {
    render(<Portfolio/>);
  });

  const headers = screen.getAllByRole("columnHeader");

  expect(headers.length).toEqual(7);
  expect(headers[0].innerHTML).toEqual('Symbol');
  expect(headers[1].innerHTML).toEqual('MKT $');
  expect(headers[2].innerHTML).toEqual('Change');
  expect(headers[3].innerHTML).toEqual('Avg $');
  expect(headers[4].innerHTML).toEqual('ROI');
  expect(headers[5].innerHTML).toEqual('Votes');
  expect(headers[6].classList.contains('portfolio-row-options-header')).toEqual(true);

  expect(screen.queryByRole('cell')).not.toBeInTheDocument();
});

it("renders single portfolio stock", () => {

    const initialPortfolio = {
      ids: [1],
      selectedPortfolioId: 1,
      portfolios: {
        1: {
          id: 1,
          portfolioSymbols: [ { id: 1, symbolId: 1, ticker: "VOO" }]
        }
      }
    };

    const initialStockData = {
      singleQuoteData: {
        1: {}
      }
    };
    
    const initialState = {
        portfolio: initialPortfolio,
        stocks: initialStockData
    };

    act(() => {
      render(<Portfolio/>, { initialState: initialState });
    });

    const headers = screen.getAllByRole("columnHeader");

    expect(headers.length).toEqual(7);
    expect(headers[0].innerHTML).toEqual('Symbol');
    expect(headers[1].innerHTML).toEqual('MKT $');
    expect(headers[2].innerHTML).toEqual('Change');1
    expect(headers[3].innerHTML).toEqual('Avg $');
    expect(headers[4].innerHTML).toEqual('ROI');
    expect(headers[5].innerHTML).toEqual('Votes');
    expect(headers[6].classList.contains('portfolio-row-options-header')).toEqual(true);
  
    expect(screen.queryByRole('cell', {name: 'VOO'})).toBeInTheDocument();  
  });

  it("renders multiple portfolio stocks", () => {
    const initialPortfolio = {
      ids: [1],
      selectedPortfolioId: 1,
      portfolios: {
        1: {
          id: 1,
          portfolioSymbols: [ { id: 1, symbolId: 1, ticker: "VOO" }, { id: 2, symbolId: 2, ticker: "AMD" }]
        }
      }
    };

    const initialStockData = {
      singleQuoteData: {
        1: {}
      }
    };
    
    const initialState = {
        portfolio: initialPortfolio,
        stocks: initialStockData
    };

    act(() => {
      render(<Portfolio/>, { initialState: initialState });
    });
  
    expect(screen.queryByRole('cell', {name: 'VOO'})).toBeInTheDocument(); 
    expect(screen.queryByRole('cell', {name: 'AMD'})).toBeInTheDocument(); 
  });


