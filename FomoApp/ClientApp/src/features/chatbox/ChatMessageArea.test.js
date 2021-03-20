import { screen, within } from '@testing-library/react';

import { ChatMessageArea } from './ChatMessageArea';
import React from 'react';
import { act } from 'react-dom/test-utils';
import { render } from '../../test-util';

const TimeFormat = { hour: 'numeric', minute:'2-digit' };
const DateFormat = { year: 'numeric', month: 'long', day: 'numeric' };

beforeEach(() => {
    window.HTMLElement.prototype.scrollIntoView = function() {};
});

afterEach(() => {
});

it("render no post text when no messages", () => {
    act(() => {
        render(<ChatMessageArea/>);
    });

    const dateSeparators = screen.queryAllByRole("heading");
    const messages = screen.queryAllByRole("log");

    expect(dateSeparators.length).toEqual(0);
    expect(messages.length).toEqual(1);

    const textField = within(messages[0]).getByText('No posts here.');
    expect(textField).toBeInTheDocument();
});

it("render date and message with text, user name, and convert timestamp to readable date", () => {

    const userName = "userName";
    const timeStampCreated = 1616197275095;
    const text = "message1";

    const chatMessages = [
        {
            id: "100",
            userName: userName,
            timeStampCreated: timeStampCreated,
            text: text
        }
    ];

    const initialState = {
        chat: {
            messageIds: ["100"],
            messages: chatMessages
        }
    };
    
    act(() => {
        render(<ChatMessageArea chatMessages={chatMessages}/>, { initialState });
    });

    const dateSeparators= screen.queryAllByRole("heading");
    const messages = screen.queryAllByRole("log");

    expect(dateSeparators.length).toEqual(1);
    expect(messages.length).toEqual(1);

    const dateCalc = new Date(timeStampCreated);
    const expectedDate = dateCalc.toLocaleDateString([], DateFormat);
    const expectedTime = dateCalc.toLocaleTimeString([], TimeFormat);

    const dateSeparatorElement = within(dateSeparators[0]);
    const messageElement = within(messages[0]);

    expect(dateSeparatorElement.getByText(expectedDate)).toBeInTheDocument();

    expect(messageElement.getByText(text)).toBeInTheDocument();
    expect(messageElement.getByText(userName)).toBeInTheDocument();
    expect(messageElement.getByText(expectedTime)).toBeInTheDocument();
});

it("renders multiple messages and group them by date", () => {

    const userName1 = "userName1";
    const userName2 = "userName2";

    const timeStampCreated1 = 1616197275095;
    const timeStampCreated2 = 1616197275105;
    const timeStampCreated3 = 1616227757401;
    const timeStampCreated4 = 1616227757501;

    const text1 = "message1";
    const text2 = "message2";
    const text3 = "message3";
    const text4 = "message4";

    const chatMessages = [
        {
            id: "100",
            userName: userName1,
            timeStampCreated: timeStampCreated1,
            text: text1
        },
        {
            id: "200",
            userName: userName2,
            timeStampCreated: timeStampCreated2,
            text: text2
        },
        {
            id: "300",
            userName: userName1,
            timeStampCreated: timeStampCreated3,
            text: text3
        },
        {
            id: "400",
            userName: userName2,
            timeStampCreated: timeStampCreated4,
            text: text4
        }
    ];

    const initialState = {
        chat: {
            messageIds: ["100", "200", "300", "400"],
            messages: chatMessages
        }
    };
    
    act(() => {
        render(<ChatMessageArea chatMessages={chatMessages}/>, { initialState });
    });

    const dateSeparators= screen.queryAllByRole("heading");
    const messages = screen.queryAllByRole("log");

    expect(dateSeparators.length).toEqual(2);
    expect(messages.length).toEqual(4);

    const dateCalc1 = new Date(timeStampCreated1);
    const dateCalc2 = new Date(timeStampCreated2);
    const dateCalc3 = new Date(timeStampCreated3);
    const dateCalc4 = new Date(timeStampCreated4);

    const expectedDate1 = dateCalc1.toLocaleDateString([], DateFormat);

    const expectedTime1 = dateCalc1.toLocaleTimeString([], TimeFormat);
    const expectedTime2 = dateCalc2.toLocaleTimeString([], TimeFormat);    
    
    const expectedDate2 = dateCalc3.toLocaleDateString([], DateFormat);

    const expectedTime3 = dateCalc3.toLocaleTimeString([], TimeFormat);
    const expectedTime4 = dateCalc4.toLocaleTimeString([], TimeFormat);

    // Check first day
    const dateSeparatorElement1 = within(dateSeparators[0]);
    expect(dateSeparatorElement1.getByText(expectedDate1)).toBeInTheDocument();

    const messageElement1 = within(messages[0]);
    expect(messageElement1.getByText(text1)).toBeInTheDocument();
    expect(messageElement1.getByText(userName1)).toBeInTheDocument();
    expect(messageElement1.getByText(expectedTime1)).toBeInTheDocument();

    const messageElement2 = within(messages[1]);
    expect(messageElement2.getByText(text2)).toBeInTheDocument();
    expect(messageElement2.getByText(userName2)).toBeInTheDocument();
    expect(messageElement2.getByText(expectedTime2)).toBeInTheDocument();

    // Check second day
    const dateSeparatorElement2 = within(dateSeparators[1]);
    expect(dateSeparatorElement2.getByText(expectedDate2)).toBeInTheDocument();

    const messageElement3 = within(messages[2]);
    expect(messageElement3.getByText(text3)).toBeInTheDocument();
    expect(messageElement3.getByText(userName1)).toBeInTheDocument();
    expect(messageElement3.getByText(expectedTime3)).toBeInTheDocument();

    const messageElement4 = within(messages[3]);
    expect(messageElement4.getByText(text4)).toBeInTheDocument();
    expect(messageElement4.getByText(userName2)).toBeInTheDocument();
    expect(messageElement4.getByText(expectedTime4)).toBeInTheDocument();
});
