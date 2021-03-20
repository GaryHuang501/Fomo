import { fireEvent, screen, waitFor, within } from '@testing-library/react';

import ChatBox from './ChatBox';
import MockFireBaseDB from '../../mocks/MockFireBaseDB';
import MockSnapshot from '../../mocks/MockSnapshot';
import React from 'react';
import { act } from 'react-dom/test-utils';
import firebase from 'firebase/app';
import { render } from '../../test-util';

beforeEach(() => {
    window.HTMLElement.prototype.scrollIntoView = function() {};
    MockFireBaseDB.ServerValue =  { TIMESTAMP : 1000 };
    firebase.database = MockFireBaseDB;
});

afterEach(() => {
    firebase.database().reset();
});

it("renders newly submitted message", async () => {

    const initialState = {
        login: {
            user: {
                id: "100"
            }
        }
    };

    act(() => {
        render(<ChatBox/>, { initialState });
    });

    const inputText = "hello!";

    const inputBox = document.getElementById('chat-input-bar-text-field');
    inputBox.innerHTML = inputText;

    //triggers input change event, inner html value is used.
    fireEvent.keyDown(inputBox,  { key: 'b', code: 'keyB' });

    const submitButton = document.getElementById('chat-input-bar-send-button');

    expect(firebase.database().refs.length).toEqual(1);
    const chatRef = firebase.database().refs[0];

    fireEvent.click(submitButton);

    await waitFor( () => expect(document.getElementById('chat-input-bar-text-field').innerHTML).toEqual(''));

    chatRef.invokeCallAllPending();

    await waitFor(() => {
        const messages = screen.getAllByRole("log");
        expect(messages.length).toEqual(1);
        expect(within(messages[0]).getByText(inputText));
    });
});

it("renders multiple loaded messages from server", async () => {
    const userId = "999";

    const initialState = {
        login: {
            user: {
                id: userId
            }
        }
    };

    const chatMessages = [
        {
            id: "100",
            userName: "user",
            timeStampCreated: 100000,
            text: "message1"
        },
        {
            id: "200",
            userName: "user",
            timeStampCreated: 100100,
            text: "message2"
        },
        {
            id: "300",
            userName: "user",
            timeStampCreated: 100200,
            text: "message3"
        }
    ];

    act(() => {
        render(<ChatBox/>, { initialState });
    });

    expect(firebase.database().refs.length).toEqual(1);
    const chatRef = firebase.database().refs[0];

    chatRef.invokeCallBack(new MockSnapshot(chatMessages[0]));
    chatRef.invokeCallBack(new MockSnapshot(chatMessages[1]));
    chatRef.invokeCallBack(new MockSnapshot(chatMessages[2]));

    await waitFor(() => expect(screen.queryAllByRole("log").length).toEqual(3));

    expect(screen.getByText("message1")).toBeInTheDocument();
    expect(screen.getByText("message2")).toBeInTheDocument();
    expect(screen.getByText("message3")).toBeInTheDocument();
});