import { fireEvent, screen } from '@testing-library/react';

import { ChatInputBar } from './ChatInputBar';
import React from 'react';
import { act } from 'react-dom/test-utils';
import { render } from '../../test-util';

beforeEach(() => {
});

afterEach(() => {
});

it("calls enter callback when enter is pressed", () => {

    let enterPressed = false;

    const onEnterPressed = (event) => {
        enterPressed = event.key === 'Enter';
    }

    act(() => {
        render(<ChatInputBar onEnterPressed={onEnterPressed} message=''/>);
    });

    const textbox = screen.getByRole("textbox");

    fireEvent.keyDown(textbox, { key: 'Enter', code: 'Enter' })

    expect(enterPressed).toEqual(true);
});

it("calls emoji callback when emoji picker is pressed", () => {

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

    expect(isEmojiPickerCalled).toEqual(true);
});

it("calls submit callback when send button is pressed", () => {
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

    expect(isSubmitPressed).toEqual(true);
});


it("renders content editable with message from props", () => {
    let isSubmitPressed;

    const onSubmitPressed = (event) => {
        isSubmitPressed = true;
    }

    act(() => {
        render(<ChatInputBar onSubmitPressed={onSubmitPressed} message='message'/>);
    });

    const textbox = screen.getByRole("textbox");
    expect(textbox.innerHTML).toEqual('message');
});
