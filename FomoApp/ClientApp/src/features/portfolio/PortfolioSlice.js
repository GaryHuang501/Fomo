import { createAsyncThunk, createSlice } from '@reduxjs/toolkit';

import axios from 'axios';

export const fetchPortfolioIds = createAsyncThunk('portfolio/fetchPortfoliIds', async (thunkApi) => {
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/portfolios`);

    return response.data;
});

export const fetchPortfolio = createAsyncThunk('portfolio/fetchPortfolio', async (id, thunkApi) => {
    const response = await axios.get(`${process.env.REACT_APP_API_URL}/portfolios/${id}`);

    return response.data;
});

export const addPortfolioStock = createAsyncThunk('portfolio/addPortfolioStock', async (payload, thunkApi) => {
    const response = await axios.post(`${process.env.REACT_APP_API_URL}/portfolios/${payload.portfolioId}/portfolioSymbols`, {symbolId: payload.symbolId});

    return response.data;
});

export const removePortfolioStock =  createAsyncThunk('portfolio/removePortfolioStock', async (payload) => {
    await axios.delete(`${process.env.REACT_APP_API_URL}/portfolios/${payload.portfolioId}/portfolioSymbols/${payload.portfolioSymbolId}`);

    return {portfolioId: payload.portfolioId, portfolioSymbolId: payload.portfolioSymbolId};
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

    await axios.patch(`${process.env.REACT_APP_API_URL}/portfolios/${portfolio.id}/portfolioSymbols/reorder`, apiPayload);

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

export const SortDirectionType = Object.freeze({ UP: -1, DOWN: 1});

export default portfolioSlice.reducer;
