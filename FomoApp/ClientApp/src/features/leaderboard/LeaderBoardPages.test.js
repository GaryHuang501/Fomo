import { screen, waitFor, within } from '@testing-library/react';

import LeaderBoardPage from './LeaderBoardPage';
import MockAdapter from 'axios-mock-adapter';
import React from 'react';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import { render } from '../../test-util';

const testUrl = "http://localhost";

let mock;
let limit;
let endPointUrl;

beforeEach(() => {
    mock = new MockAdapter(axios);

    process.env = {
        REACT_APP_CHART_URL: testUrl,
        REACT_APP_API_LEADERBOARD_LIMIT: 100
    };

    limit = 5;

    endPointUrl = `${process.env.REACT_APP_API_URL}/leaderboard?limit=${process.env.REACT_APP_API_LEADERBOARD_LIMIT}`
});

afterEach(() => {
    mock.restore();
    process.env = {};
});


it("can handle rendering initial empty state leader board data", async () => {

    const leaderBoardData = {
        mostBullish: null,
        mostBearish: null,
        bestPerformers: null,
        worstPerformers: null
    };

    mock.onGet(endPointUrl)
        .reply(200, leaderBoardData);

    act(() => {
        render(<LeaderBoardPage />);
    });

    Promise.resolve();

    await waitFor(() => expect(screen.getByRole("main")).toBeInTheDocument());
});


it("renders most bullish, most bearish, best performer, and worst performer boarder.", async () => {

    const leaderBoardData = {
        mostBullish: {
            header: "Most Bullish Stocks",
            columnHeaderName: "Ticker",
            columnHeaderValue: "Votes",
            values: [
                {id: 1, name: "JPM", value: 5},
                {id: 2, name: "BAC", value: 4},
            ]
        },
        mostBearish: {
            header: "Most Bearish Stocks",
            columnHeaderName: "Ticker",
            columnHeaderValue: "Votes",
            values: [
                {id: 3, name: "TSLA", value: -10},
                {id: 4, name: "GME", value: -9},
            ]
        },
        bestPerformers: {
            header: "Best Performers",
            columnHeaderName: "User",
            columnHeaderValue: "ROI %",
            values: [
                {id: 1, name: "George", value: "50"},
                {id: 2, name: "Joe", value: "10"},
            ]
        },
        worstPerformers: {
            header: "Worst Performers",
            columnHeaderName: "User",
            columnHeaderValue: "ROI %",
            values: [
                {id: 3, name: "Timmy", value: "-50"},
                {id: 4, name: "Jimmy", value: "-10"},
            ]
        },
    };

    mock.onGet(endPointUrl)
        .reply(200, leaderBoardData);

    act(() => {
        render(<LeaderBoardPage />);
    });

    Promise.resolve();

    await waitFor(() => expect(screen.queryAllByRole("table").length).toBe(4));

    const leaderBoards = screen.queryAllByRole("table");

    // Check most bullish leaderboard
    expect(within(leaderBoards[0]).getByRole("heading", {level: 3, name: "Most Bullish Stocks"})).toBeInTheDocument();

    const mostBullishRows = within(leaderBoards[0]).queryAllByRole("row");
    expect(mostBullishRows.length).toBe(3);

    expect(within(mostBullishRows[0]).getByRole("columnheader", {name: "Ticker"})).toBeInTheDocument();
    expect(within(mostBullishRows[0]).getByRole("columnheader", {name: "Votes"})).toBeInTheDocument();

    expect(within(mostBullishRows[1]).getByText("JPM"));
    expect(within(mostBullishRows[1]).getByText("5"));

    expect(within(mostBullishRows[2]).getByText("BAC"));
    expect(within(mostBullishRows[2]).getByText("4"));

    // Check most bearish leaderboard
    expect(within(leaderBoards[1]).getByRole("heading", {level: 3, name: "Most Bearish Stocks"})).toBeInTheDocument();

    const mostBearishRows = within(leaderBoards[1]).queryAllByRole("row");
    expect(mostBearishRows.length).toBe(3);

    expect(within(mostBearishRows[0]).getByRole("columnheader", {name: "Ticker"})).toBeInTheDocument();
    expect(within(mostBearishRows[0]).getByRole("columnheader", {name: "Votes"})).toBeInTheDocument();

    expect(within(mostBearishRows[1]).getByText("TSLA"));
    expect(within(mostBearishRows[1]).getByText("-10"));

    expect(within(mostBearishRows[2]).getByText("GME"));
    expect(within(mostBearishRows[2]).getByText("-9"));

    // Check best performers leaderboard
    expect(within(leaderBoards[2]).getByRole("heading", {level: 3, name: "Best Performers"})).toBeInTheDocument();

    const bestPerformerRows = within(leaderBoards[2]).queryAllByRole("row");
    expect(bestPerformerRows.length).toBe(3);

    expect(within(bestPerformerRows[0]).getByRole("columnheader", {name: "User"})).toBeInTheDocument();
    expect(within(bestPerformerRows[0]).getByRole("columnheader", {name: "ROI %"})).toBeInTheDocument();

    expect(within(bestPerformerRows[1]).getByText("George"));
    expect(within(bestPerformerRows[1]).getByText("50"));

    expect(within(bestPerformerRows[2]).getByText("Joe"));
    expect(within(bestPerformerRows[2]).getByText("10"));

    // Check worst Performers
    expect(within(leaderBoards[3]).getByRole("heading", {level: 3, name: "Worst Performers"})).toBeInTheDocument();

    const worstPerformerRows = within(leaderBoards[3]).queryAllByRole("row");
    expect(worstPerformerRows.length).toBe(3);

    expect(within(worstPerformerRows[0]).getByRole("columnheader", {name: "User"})).toBeInTheDocument();
    expect(within(worstPerformerRows[0]).getByRole("columnheader", {name: "ROI %"})).toBeInTheDocument();

    expect(within(worstPerformerRows[1]).getByText("Timmy"));
    expect(within(worstPerformerRows[1]).getByText("-50"));

    expect(within(worstPerformerRows[2]).getByText("Jimmy"));
    expect(within(worstPerformerRows[2]).getByText("-10"));
});
