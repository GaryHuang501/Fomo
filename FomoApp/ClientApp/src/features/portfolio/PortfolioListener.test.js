import 'firebase/auth';
import 'firebase/database';
import 'firebase/analytics';

import { screen, waitFor, within } from '@testing-library/react';

import MockAdapter from 'axios-mock-adapter';
import MockFireBaseDB from '../../mocks/MockFireBaseDB';
import MockSnapshot from '../../mocks/MockSnapshot';
import PortfolioListener from './PortfolioListener';
import { PortfolioStock } from './PortfolioStock';
import React from 'react';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import firebase from 'firebase/app';
import { render } from '../../test-util';
import { singleQuoteDataPath } from '../../app/FireBasePaths';

let mock;

beforeEach(() => {
    jest.useFakeTimers();

    firebase.database = MockFireBaseDB;
    mock = new MockAdapter(axios);

    process.env = {
        REACT_APP_STOCK_REFRESH_RATE_MS: 10000, // make it long so test manually trigger the refresh
        REACT_APP_STOCK_SUBSCRIBE_RATE_MS: 20000,
        REACT_APP_API_URL: "http:localhost"
    };
});

afterEach(() => {
    jest.clearAllTimers();
    jest.restoreAllMocks();
    firebase.database().reset();
    mock.restore();
});

function getRefPath(symbolId){
    return `${singleQuoteDataPath}/${symbolId}`;
}

it("Should fetch stock data for single symbol in portfoio immediately when loaded", async () => {

    const portfolioSymbol = { id: 1, symbolId: 1, ticker: "VOO" };

    const initialPortfolio = {
        ids: [1],
        selectedPortfolioId: 1,
        portfolios: {
            1: {
                id: 1,
                portfolioSymbols: [portfolioSymbol]
            }
        }
    };

    const initialStockData = {
        singleQuoteData: {
        }
    };

    const initialState = {
        portfolio: initialPortfolio,
        stocks: initialStockData
    };

    const singleQuoteData = {
        symbolId: 1,
        lastUpdated: null,
        data:
            {
                symbolId: portfolioSymbol.symbolId,
                ticker: portfolioSymbol.ticker,
                price: 12.15,
                averagePrice: 12.50,
                changePercent: 12.50,
                votes: 5,
                return: 1,
                lastUpdated: '2020-01-01'
            }
    };

    const mock = new MockAdapter(axios);
    mock.onGet(`${process.env.REACT_APP_API_URL}/singleQuoteData?sids=1`)
        .reply(200, [singleQuoteData]);

    const spy = jest.spyOn(axios, 'get');

    act(() => {
        render(<div>
                    <PortfolioListener />
                    <table>
                        <tbody><PortfolioStock key={portfolioSymbol.symbolId} portfolioSymbol={portfolioSymbol}/></tbody>
                    </table>
                </div>,         
            { initialState });
    });

    await Promise.resolve();

    expect(spy).toHaveBeenCalledTimes(1);
        
    await waitFor(() => {
        expect(screen.getByText(singleQuoteData.data.ticker)).toBeInTheDocument();
        expect(screen.getByText(singleQuoteData.data.price.toString())).toBeInTheDocument();
    });
});

it("Should not fetch stock data on batch call when no new notification updates", async () => {

    const portfolioSymbol = { id: 1, symbolId: 1, ticker: "VOO" };

    const initialPortfolio = {
        ids: [1],
        selectedPortfolioId: 1,
        portfolios: {
            1: {
                id: 1,
                portfolioSymbols: [portfolioSymbol]
            }
        }
    };

    const initialStockData = {
        singleQuoteData: {
            1: {
                lastUpdated: '2020-01-01',
                symbolId: portfolioSymbol.symbolId,
                ticker: portfolioSymbol.ticker
            }
        }
    };

    const initialState = {
        portfolio: initialPortfolio,
        stocks: initialStockData
    };

    const singleQuoteData = {
        symbolId: 1,
        lastUpdated: '2020-01-01',
        data:
            {
                symbolId: portfolioSymbol.symbolId,
                ticker: portfolioSymbol.ticker,
                price: 12.15,
                averagePrice: 12.50,
                changePercent: 12.50,
                votes: 5,
                return: 1,
                lastUpdated: '2020-01-01'
            }
    };

    const mock = new MockAdapter(axios);
    mock.onGet(`${process.env.REACT_APP_API_URL}/singleQuoteData?sids=1`)
        .reply(200, [singleQuoteData]);

    const spy = jest.spyOn(axios, 'get');

    act(() => {
        render(<div>
                    <PortfolioListener />
                    <table>
                        <tbody><PortfolioStock key={portfolioSymbol.symbolId} portfolioSymbol={portfolioSymbol}/></tbody>
                    </table>
                </div>,         
            { initialState });
    });

    await Promise.resolve();

    jest.clearAllMocks();
    
    // Notification should do nothing since notification isn't newer
    for(const ref of firebase.database().refs)
    {
        ref.invokeCallBack(new MockSnapshot({lastUpdated: '2020-01-01'}));
    }
    
    jest.advanceTimersByTime(10000);

    await Promise.resolve();

    expect(spy).toHaveBeenCalledTimes(0);
    
    await waitFor(() => {
        expect(screen.getByText(singleQuoteData.data.ticker)).toBeInTheDocument();
    });
});

it("Should fetch stock data for multiple symbol in portfoio on batch call", async () => {

    const portfolioSymbol1 = { id: 1, symbolId: 1, ticker: "VOO" };
    const portfolioSymbol2 = { id: 2, symbolId: 2, ticker: "VTI" };

    const initialPortfolio = {
        ids: [1],
        selectedPortfolioId: 1,
        portfolios: {
            1: {
                id: 1,
                portfolioSymbols: [portfolioSymbol1, portfolioSymbol2]
            }
        }
    };

    const initialStockData = {
        singleQuoteData: {
        }
    };

    const initialState = {
        portfolio: initialPortfolio,
        stocks: initialStockData
    };

    const singleQuoteData1 = {
        symbolId: portfolioSymbol1.symbolId,
        lastUpdated: null,
        data:
            {
                symbolId: portfolioSymbol1.symbolId,
                ticker: portfolioSymbol1.ticker,
                price: 12.15,
                averagePrice: 12.50,
                changePercent: 12.50,
                votes: 5,
                return: 1,
                lastUpdated: '2020-01-01'
            }
    };

    const singleQuoteData2 = {
        symbolId: portfolioSymbol2.symbolId,
        lastUpdated: null,
        data:
            {
                symbolId: portfolioSymbol2.symbolId,
                ticker: portfolioSymbol2.ticker,
                price: 100.75,
                averagePrice: 3.50,
                changePercent: 88.50,
                votes: 3,
                return: 1,
                lastUpdated: '2020-01-01'
            }
    };


    mock.onGet(`${process.env.REACT_APP_API_URL}/singleQuoteData?sids=1&sids=2`)        
        .reply(200, [singleQuoteData1, singleQuoteData2]);
        
    const spy = jest.spyOn(axios, 'get');

    act(() => {
        render(<div>
                    <PortfolioListener />
                    <table>
                        <tbody>
                            <PortfolioStock key={portfolioSymbol1.symbolId} portfolioSymbol={portfolioSymbol1}/>
                            <PortfolioStock key={portfolioSymbol2.symbolId} portfolioSymbol={portfolioSymbol2}/>
                        </tbody>
                    </table>
                </div>,         
            { initialState });
    });

    await Promise.resolve();

    jest.clearAllMocks();
    
    for(const ref of firebase.database().refs)
    {
        ref.invokeCallBack(new MockSnapshot({lastUpdated: '2020-01-01'}));
    }

    jest.advanceTimersByTime(20000);

    await Promise.resolve();

    expect(spy.mock.calls[0][0]).toEqual(`${process.env.REACT_APP_API_URL}/singleQuoteData?sids=1&sids=2`);

    await waitFor(() => {
        expect(screen.getByText(singleQuoteData1.data.ticker)).toBeInTheDocument();
        expect(screen.getByText(singleQuoteData1.data.price.toString())).toBeInTheDocument();

        expect(screen.getByText(singleQuoteData2.data.ticker)).toBeInTheDocument();
        expect(screen.getByText(singleQuoteData2.data.price.toString())).toBeInTheDocument();
    });
});

it("Should only update stocks if the cached stock last updated date is older than the notifcation", async () => {

    const portfolioSymbol1 = { id: 1, symbolId: 1, ticker: "VOO" };
    const portfolioSymbol2 = { id: 2, symbolId: 2, ticker: "VTI" };

    const initialPortfolio = {
        ids: [1],
        selectedPortfolioId: 1,
        portfolios: {
            1: {
                id: 1,
                portfolioSymbols: [portfolioSymbol1, portfolioSymbol2]
            }
        }
    };

    const singleQuoteData1 = {
        symbolId: portfolioSymbol1.symbolId,
        lastUpdated: '2020-01-01',
        data:
            {
                symbolId: portfolioSymbol1.symbolId,
                ticker: portfolioSymbol1.ticker,
                price: 12.15,
                averagePrice: 12.50,
                changePercent: 12.50,
                votes: 5,
                return: 1,
                lastUpdated: '2020-01-01'
            }
    };

    const singleQuoteData2 = {
        symbolId: portfolioSymbol2.symbolId,
        lastUpdated: '2020-01-01',
        data:
            {
                symbolId: portfolioSymbol2.symbolId,
                ticker: portfolioSymbol2.ticker,
                price: 5.99,
                averagePrice: 1.25,
                changePercent: 1,
                votes: 1,
                return: 1,
                lastUpdated: '2020-01-01',
            }
    };

    const initialStockData = {
        singleQuoteData: {
            [portfolioSymbol1.symbolId]: singleQuoteData1.data,
            [portfolioSymbol2.symbolId]: singleQuoteData2.data
        }
    };

    const initialState = {
        portfolio: initialPortfolio,
        stocks: initialStockData
    };

    const updateSingleQuoteData1 = {
        symbolId: portfolioSymbol1.symbolId,
        lastUpdated: '2021-01-01',
        data:
            {
                symbolId: portfolioSymbol1.symbolId,
                ticker: portfolioSymbol1.ticker,
                price: 12.92,
                averagePrice: 12.50,
                changePercent: 12.50,
                votes: 5,
                return: 1,
                lastUpdated: '2021-01-01'
            }
    };

    mock.onGet(`${process.env.REACT_APP_API_URL}/singleQuoteData?sids=1`)        
        .reply(200, [updateSingleQuoteData1]);
        
    const spy = jest.spyOn(axios, 'get');

    act(() => {
        render(<div>
                    <PortfolioListener />
                    <table>
                        <tbody>
                            <PortfolioStock key={portfolioSymbol1.symbolId} portfolioSymbol={portfolioSymbol1}/>
                            <PortfolioStock key={portfolioSymbol2.symbolId} portfolioSymbol={portfolioSymbol2}/>
                        </tbody>
                    </table>
                </div>,         
            { initialState });
    });

    await Promise.resolve();
    
    jest.clearAllMocks();
    
    const symbol1Ref = firebase.database().refs.find(r => r.path === getRefPath(portfolioSymbol1.symbolId));
    const symbol2Ref = firebase.database().refs.find(r => r.path === getRefPath(portfolioSymbol2.symbolId));

    symbol1Ref.invokeCallBack(new MockSnapshot({lastUpdated: '2021-01-01'}));
    symbol2Ref.invokeCallBack(new MockSnapshot({lastUpdated: '2019-01-01'}));
 
    jest.advanceTimersByTime(10000);

    await Promise.resolve();

    expect(spy.mock.calls[0][0]).toEqual(`${process.env.REACT_APP_API_URL}/singleQuoteData?sids=1`);

    await waitFor(() => {
        var portfolioStock = screen.getAllByRole('row');

        expect(within(portfolioStock[0]).getByText(updateSingleQuoteData1.data.ticker)).toBeInTheDocument();
        expect(within(portfolioStock[0]).getByText(updateSingleQuoteData1.data.price.toString())).toBeInTheDocument();

        expect(within(portfolioStock[1]).getByText(singleQuoteData2.data.ticker)).toBeInTheDocument();
        expect(within(portfolioStock[1]).getByText(singleQuoteData2.data.price)).toBeInTheDocument();
    });
});

it("Should not update stock if server data is older than cached in store", async () => {

    const portfolioSymbol1 = { id: 1, symbolId: 1, ticker: "VOO" };

    const initialPortfolio = {
        ids: [1],
        selectedPortfolioId: 1,
        portfolios: {
            1: {
                id: 1,
                portfolioSymbols: [portfolioSymbol1]
            }
        }
    };

    const originalQuoteData = {
        symbolId: portfolioSymbol1.symbolId,
        lastUpdated: '2020-01-01',
        data:
            {
                symbolId: portfolioSymbol1.symbolId,
                ticker: portfolioSymbol1.ticker,
                price: 12.15,
                averagePrice: 12.50,
                changePercent: 12.50,
                votes: 5,
                return: 1,
                lastUpdated: '2020-01-01'
            }
    };

    const initialStockData = {
        singleQuoteData: {
            [portfolioSymbol1.symbolId]: originalQuoteData.data,
        }
    };

    const initialState = {
        portfolio: initialPortfolio,
        stocks: initialStockData
    };

    const serverSingleQuoteData1 = {
        symbolId: portfolioSymbol1.symbolId,
        lastUpdated: '2019-01-01',
        data:
            {
                symbolId: portfolioSymbol1.symbolId,
                ticker: portfolioSymbol1.ticker,
                price: 12.92,
                averagePrice: 12.50,
                changePercent: 12.50,
                votes: 5,
                return: 1,
                lastUpdated: '2019-01-01'
            }
    };

    mock.onGet(`${process.env.REACT_APP_API_URL}/singleQuoteData?sids=1`)        
        .reply(200, [serverSingleQuoteData1]);
        
    const spy = jest.spyOn(axios, 'get');

    act(() => {
        render(<div>
                    <PortfolioListener />
                    <table>
                        <tbody>
                            <PortfolioStock key={portfolioSymbol1.symbolId} portfolioSymbol={portfolioSymbol1}/>
                        </tbody>
                    </table>
                </div>,         
            { initialState });
    });

    await Promise.resolve();
       
    const symbol1Ref = firebase.database().refs.find(r => r.path === getRefPath(portfolioSymbol1.symbolId));
    symbol1Ref.invokeCallBack(new MockSnapshot({lastUpdated: '2021-01-01'}));
 
    jest.advanceTimersByTime(20000);

    await Promise.resolve();

    expect(spy.mock.calls[0][0]).toEqual(`${process.env.REACT_APP_API_URL}/singleQuoteData?sids=1`);

    await waitFor(() => {
        var portfolioStock = screen.getAllByRole('row');

        expect(within(portfolioStock[0]).getByText(originalQuoteData.data.ticker)).toBeInTheDocument();
        expect(within(portfolioStock[0]).getByText(originalQuoteData.data.price.toString())).toBeInTheDocument();
    });
});

it("Should not fetch stock data when empty portfolio", async () => {

    const initialPortfolio = {
        ids: [1],
        selectedPortfolioId: 1,
        portfolios: {
            1: {
                id: 1,
                portfolioSymbols: []
            }
        }
    };

    const initialStockData = {
        singleQuoteData: {
        }
    };

    const initialState = {
        portfolio: initialPortfolio,
        stocks: initialStockData
    };

    const mock = new MockAdapter(axios);
    const spy = jest.spyOn(axios, 'get');

    act(() => {
        render(<div>
                    <PortfolioListener/>
                </div>,         
            { initialState });
    });

    await Promise.resolve();

    expect(spy).not.toHaveBeenCalled();
    
    for(const ref of firebase.database().refs)
    {
        ref.invokeCallBack(new MockSnapshot(null));
    }
    
    jest.advanceTimersByTime(20000);

    await Promise.resolve();

    expect(spy).not.toHaveBeenCalled();
});