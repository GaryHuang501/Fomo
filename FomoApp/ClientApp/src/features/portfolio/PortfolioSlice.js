import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';
import { sendHistory } from '../chatbox/ChatSlice'

export const fetchPortfolioIds = createAsyncThunk('portfolio/fetchPortfoliIds', async (userId, thunkApi) => {
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/portfolios?userId=${userId}`);

    return response.data;
});

export const fetchPortfolio = createAsyncThunk('portfolio/fetchPortfolio', async (id, thunkApi) => {
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/portfolios/${id}`);

    return response.data;
});

export const addPortfolioStock = createAsyncThunk('portfolio/addPortfolioStock', async (payload, thunkApi) => {
    const response = await axios.post(`${process.env.REACT_APP_API_URL}/portfolios/${payload.portfolioId}/portfolioSymbols`, {symbolId: payload.symbolId});

    thunkApi.dispatch(sendHistory(`Added ${response.data.ticker} to portfolio`));

    return response.data;
});

export const updateAvergePricePortfolioStock =  createAsyncThunk('portfolio/updateAvgPricePortfolioStock', async (payload, thunkApi) => {

    const portfolio = selectSelectedPortfolio(thunkApi.getState());
    const endPointUrl = `${process.env.REACT_APP_API_URL}/portfolios/${portfolio.id}/portfolioSymbols/${payload.portfolioSymbolId}`;
    await axios.patch(endPointUrl, [{ op: "replace", path: "/averagePrice", value: payload.averagePrice}]);

    const portfolioSymbol = portfolio.portfolioSymbols.find(s => s.id === payload.portfolioSymbolId);

    thunkApi.dispatch(sendHistory(`Updated average price of ${portfolioSymbol.ticker} to ${payload.averagePrice}`));

    return {portfolioId: portfolio.id, portfolioSymbolId: payload.portfolioSymbolId, averagePrice: parseFloat(payload.averagePrice)};
});

export const removePortfolioStock =  createAsyncThunk('portfolio/removePortfolioStock', async (payload, thunkApi) => {
    const portfolio = selectSelectedPortfolio(thunkApi.getState());

    await axios.delete(`${process.env.REACT_APP_API_URL}/portfolios/${portfolio.id}/portfolioSymbols/${payload.portfolioSymbolId}`);

    const portfolioSymbol = portfolio.portfolioSymbols.find(s => s.id === payload.portfolioSymbolId);

    thunkApi.dispatch(sendHistory(`${portfolioSymbol.ticker} was removed from portfolio`));

    return {portfolioId: portfolio.id, portfolioSymbolId: payload.portfolioSymbolId};
});

export const movePortfolioStock =  createAsyncThunk('portfolio/movePortfolioStock', async (payload, thunkApi) => {

    const { portfolioSymbolId, sortDirection}  = payload;

    const portfolio = selectSelectedPortfolio(thunkApi.getState());

    const index = portfolio.portfolioSymbols.findIndex(s => s.id === portfolioSymbolId)

    if(index < 0) return;

    const newIndex = index + sortDirection;

    if(newIndex < 0 || newIndex > portfolio.portfolioSymbols.length) return;

    const newSortedArray = portfolio.portfolioSymbols.slice();

    const temp = newSortedArray[index];
    
    newSortedArray[index] = newSortedArray[newIndex];
    newSortedArray[newIndex] = temp;

    const portfolioSymbolIdToSortOrder = {}

    newSortedArray.forEach((ps, index) => {
        portfolioSymbolIdToSortOrder[ps.id] = index; 
    });

    const apiPayload = { portfolioSymbolIdToSortOrder: Object.assign({}, portfolioSymbolIdToSortOrder) }

    await axios.patch(`${process.env.REACT_APP_API_URL}/portfolios/${portfolio.id}/portfolioSymbols/sortOrder`, apiPayload);

    return newSortedArray;
});

export const portfolioSlice = createSlice({
    name: 'portfolio',
    initialState: {
        ids: [],
        selectedPortfolioId: null,
        portfolios: {

        },
        fetchIdStatus: "idle",
        fetchPortfolioStatus: "idle",
        addPortfolioSymbolStatus: "idle"
    },
    reducers: {
        setSelectedPortfolioId: (state, action) => {
            state.selectedPortfolioId = action.payload;
        }
    },
    extraReducers: {
        [fetchPortfolioIds.pending]: (state, action) => {
            state.fetchIdStatus = 'loading';
        },
        [fetchPortfolioIds.fulfilled]: (state, action) => {
            state.fetchIdStatus = 'succeeded';
            state.ids = action.payload ?? [];
        },
        [fetchPortfolioIds.rejected]: (state, action) => {
            state.fetchIdStatus = 'failed';
        },
        [fetchPortfolio.pending]: (state, action) => {
            state.fetchPortfolioStatus = 'loading';
        },
        [fetchPortfolio.fulfilled]: (state, action) => {
            state.fetchPortfolioStatus = 'succeeded';
            state.portfolios[action.meta.arg] = action.payload;
        },
        [fetchPortfolio.rejected]: (state, action) => {
            state.fetchPortfolioStatus = 'failed';
        },
        [addPortfolioStock.pending]: (state, action) => {
            state.addPortfolioSymbolStatus = 'loading';
        },
        [addPortfolioStock.fulfilled]: (state, action) => {
            state.addPortfolioSymbolStatus = 'succeeded';
            state.portfolios[state.selectedPortfolioId].portfolioSymbols.push(action.payload);
        },
        [addPortfolioStock.rejected]: (state, action) => {
            state.addPortfolioSymbolStatus = 'rejected';
        },
        [updateAvergePricePortfolioStock.fulfilled]: (state, action) => {
            const portfolioSymbol = state.portfolios[state.selectedPortfolioId].portfolioSymbols.find(s => s.id === action.payload.portfolioSymbolId);

            if(portfolioSymbol){
                portfolioSymbol.averagePrice = action.payload.averagePrice;
            }
        },
        [removePortfolioStock.fulfilled]: (state, action) => {
            const portfolioSymbolIdToRemove = action.payload.portfolioSymbolId;
            const portfolio = state.portfolios[state.selectedPortfolioId]
            portfolio.portfolioSymbols = portfolio.portfolioSymbols.filter(s => s.id !== portfolioSymbolIdToRemove);
        },
        [movePortfolioStock.fulfilled]: (state, action) => {
            state.portfolios[state.selectedPortfolioId].portfolioSymbols = action.payload;
        }
    } 
});

export const { setSelectedPortfolioId } = portfolioSlice.actions;

export const selectPortfolioIds = state => state.portfolio.ids;

export const selectSelectedPortfolioId = state => state.portfolio.selectedPortfolioId;

export const selectSelectedPortfolio = function(state){  
    const portfolio = state.portfolio.portfolios[state.portfolio.selectedPortfolioId];
    return portfolio ? portfolio : { id: -1, portfolioSymbols: [] };
}

export const selectSelectedPortfolioSymbols = function(state) {

    if(!state.portfolio.selectedPortfolioId || !(state.portfolio.selectedPortfolioId in state.portfolio.portfolios)){
        return [];
    }

    return state.portfolio.portfolios[state.portfolio.selectedPortfolioId].portfolioSymbols;
}

export const SortDirectionType = Object.freeze({ UP: -1, DOWN: 1});

export default portfolioSlice.reducer;
