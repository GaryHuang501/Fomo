import 'firebase/auth';
import 'firebase/database';
import 'firebase/analytics';

import { fireEvent, screen, waitFor, within } from '@testing-library/react';

import MockAdapter from 'axios-mock-adapter';
import { PortfolioListener } from './PortfolioListener';
import { PortfolioStock } from './PortfolioStock';
import React from 'react';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import firebase from 'firebase/app';
import { render } from '../../test-util';

class MockFireBaseRef {

    constructor(path) { 
        const parts = path.split('/');
        this.symbolId = parts[1];
    }

    on(event, callback) {
        this.onEvent = event;
        this.callback = callback;
    }

    off(event){
        this.offEvent = event;
    }

    invokeCallBack(snapshot){
        this.callback(snapshot);
    }
}

class Snapshot{

    constructor(value){
        this.value = value;
    }

    val(){
        return this.value;
    }

    test(){
        return 'yo';
    }
}

let firebaseRefs = [];

const mockFireBaseDB = function (){

    this.ref = function(path) {
        const ref = new MockFireBaseRef(path);
        firebaseRefs.push(ref);
        return ref;
    }

    return this;
};

let mock;

beforeEach(() => {
    jest.useFakeTimers();

    firebaseRefs = [];
    firebase.database = mockFireBaseDB;
    
    mock = new MockAdapter(axios);

    process.env = {
        REACT_APP_STOCK_REFRESH_RATE_MS: 10000, // make it long so test manually trigger the refresh
        REACT_APP_API_URL: "http:localhost"
    };
});

afterEach(() => {
    jest.clearAllTimers();
    jest.restoreAllMocks();
    mock.restore();
});

it("Should fetch stock data for single symbol in portfoio", async () => {

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
    mock.onGet(`${process.env.REACT_APP_API_URL}/singleQuoteData?symbolIds=1`)
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

    expect(spy).not.toHaveBeenCalled();
    
    for(const ref of firebaseRefs)
    {
        ref.invokeCallBack(new Snapshot(null));
    }
    
    jest.advanceTimersByTime(20000);

    await Promise.resolve();

    expect(spy).toHaveBeenCalledTimes(1);
    
    await waitFor(() => {
        expect(screen.getByText(singleQuoteData.data.ticker)).toBeInTheDocument();
        expect(screen.getByText(singleQuoteData.data.price.toString())).toBeInTheDocument();
    });
});

it("Should fetch stock data for multiple symbol in portfoio", async () => {

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


    mock.onGet(`${process.env.REACT_APP_API_URL}/singleQuoteData?symbolIds=1&symbolIds=2`)        
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

    expect(spy).not.toHaveBeenCalled();
    
    for(const ref of firebaseRefs)
    {
        ref.invokeCallBack(new Snapshot(null));
    }

    jest.advanceTimersByTime(20000);

    await Promise.resolve();

    expect(spy).toHaveBeenCalledTimes(1);
    expect(spy.mock.calls[0][0]).toEqual(`${process.env.REACT_APP_API_URL}/singleQuoteData?symbolIds=1&symbolIds=2`);

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

    mock.onGet(`${process.env.REACT_APP_API_URL}/singleQuoteData?symbolIds=1`)        
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

    expect(spy).not.toHaveBeenCalled();
    
        
    const symbol1Ref = firebaseRefs.find(r => r.symbolId == portfolioSymbol1.symbolId);
    const symbol2Ref = firebaseRefs.find(r => r.symbolId == portfolioSymbol2.symbolId);

    symbol1Ref.invokeCallBack(new Snapshot({lastUpdated: '2021-01-01'}));
    symbol2Ref.invokeCallBack(new Snapshot({lastUpdated: '2019-01-01'}));
 
    jest.advanceTimersByTime(20000);

    await Promise.resolve();

    expect(spy).toHaveBeenCalledTimes(1);
    expect(spy.mock.calls[0][0]).toEqual(`${process.env.REACT_APP_API_URL}/singleQuoteData?symbolIds=1`);

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

    const initialStockData = {
        singleQuoteData: {
            [portfolioSymbol1.symbolId]: singleQuoteData1.data,
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

    mock.onGet(`${process.env.REACT_APP_API_URL}/singleQuoteData?symbolIds=1`)        
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

    expect(spy).not.toHaveBeenCalled();
          
    const symbol1Ref = firebaseRefs.find(r => r.symbolId == portfolioSymbol1.symbolId);
    symbol1Ref.invokeCallBack(new Snapshot({lastUpdated: '2021-01-01'}));
 
    jest.advanceTimersByTime(20000);

    await Promise.resolve();

    expect(spy).toHaveBeenCalledTimes(1);
    expect(spy.mock.calls[0][0]).toEqual(`${process.env.REACT_APP_API_URL}/singleQuoteData?symbolIds=1`);

    await waitFor(() => {
        var portfolioStock = screen.getAllByRole('row');

        expect(within(portfolioStock[0]).getByText(singleQuoteData1.data.ticker)).toBeInTheDocument();
        expect(within(portfolioStock[0]).getByText(singleQuoteData1.data.price.toString())).toBeInTheDocument();
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
    
    for(const ref of firebaseRefs)
    {
        ref.invokeCallBack(new Snapshot(null));
    }
    
    jest.advanceTimersByTime(20000);

    await Promise.resolve();

    expect(spy).not.toHaveBeenCalled();
});