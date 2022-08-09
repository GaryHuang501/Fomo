import { fireEvent, screen, waitFor } from '@testing-library/react';

import { ChatInputBar } from './ChatInputBar';
import React from 'react';
import { act } from 'react-dom/test-utils';
import { render } from '../../test-util';

beforeEach(() => {
});

afterEach(() => {
});

it("calls enter callback when enter is pressed", async () => {

    let enterPressed = false;

    const onEnterPressed = (event) => {
        enterPressed = event.key === 'Enter';
    }

    act(() => {
        render(<ChatInputBar onEnterPressed={onEnterPressed} message=''/>);
    });

    const textbox = screen.getByRole("textbox");

    fireEvent.keyDown(textbox, { key: 'Enter', code: 'Enter' })

    await waitFor(() => expect(enterPressed).toEqual(true));
});

it("calls emoji callback when emoji picker is pressed", async() => {

    let isEmojiPickerCalled;

    const onEmojiPickerPressed = (event) => {
        isEmojiPickerCalled = true;
    }

    act(() => {
        render(<ChatInputBar onShowEmojiPicker={onEmojiPickerPressed} message=''/>);
    });

    const buttons = screen.getAllByRole("button");
    const emojiButton = buttons.find(p => p.classList.contains("fa-smile"));

    fireEvent.click(emojiButton);

    await waitFor(() => expect(isEmojiPickerCalled).toEqual(true));
});

it("calls submit callback when send button is pressed", async() => {
    let isSubmitPressed;

    const onSubmitPressed = (event) => {
        isSubmitPressed = true;
    }

    act(() => {
        render(<ChatInputBar onSubmitPressed={onSubmitPressed} message=''/>);
    });

    const buttons = screen.getAllByRole("button");
    const submitButton = buttons.find(p => p.classList.contains("fa-sign-in-alt"));

    fireEvent.click(submitButton);

    waitFor(() => expect(isSubmitPressed).toEqual(true));
});


it("renders content editable with message from props", async() => {
    let isSubmitPressed;

    const onSubmitPressed = (event) => {
        isSubmitPressed = true;
    }

    act(() => {
        render(<ChatInputBar onSubmitPressed={onSubmitPressed} message='message'/>);
    });

    const textbox = screen.getByRole("textbox");
    await waitFor(() => expect(textbox.innerHTML).toEqual('message'));
});
