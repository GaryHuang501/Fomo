import { fireEvent, screen, waitFor } from '@testing-library/react';

import React from 'react';
import VoteColumn from './VoteColumn';
import { act } from 'react-dom/test-utils';
import { render } from '../../test-util';

const positiveDeltaClass = "portfolio-stock-positive-delta";
const negativeDeltaClass = "portfolio-stock-negative-delta";

beforeEach(() => {
});

afterEach(() => {
});


it("renders votes count as 0 when no votes found", async () => {

  const props  = {
  }
  act(() => {
    render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={false}/></tr></tbody></table>);
  });

  const voteFields = document.getElementsByClassName("portfolio-row-votes-value")

  expect(voteFields.length).toEqual(1);
  expect(voteFields[0].innerHTML.trim()).toEqual("0");
});

it("renders upvote button as green rocket ship when votes > 0", async () => {
    const props  = {
        votes: 1,
        myVoteDir: 0
    };

    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={false}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];
    const downVoteButton = voteButtons[1];

    expect(upVoteButton.classList).toContain(positiveDeltaClass);
    expect(downVoteButton.classList).toContain("hidden");
});

it("renders downvote button as red rocket ship when votes < 0", async () => {
    const props  = {
        votes: -1,
        myVoteDir: 0
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={false}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];
    const downVoteButton = voteButtons[1];

    expect(downVoteButton.classList).toContain(negativeDeltaClass);
    expect(upVoteButton.classList).toContain("hidden");
});

it("renders both vote buttons normally when votes is 0", async () => {
    const props  = {
        votes: 0,
        myVoteDir: 0
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={false}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];
    const downVoteButton = voteButtons[1];

    expect(downVoteButton.classList).not.toContain("hidden");
    expect(upVoteButton.classList).not.toContain("hidden");
});

it("renders the upvote icon as highlighted when I upvoted before in edit mode", async () => {
    const props  = {
        votes: 1,
        myVoteDir: 1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];

    expect(upVoteButton.classList).toContain("already-voted");
});

it("renders the buttons as clickable when in edit mode", async () => {
    const props  = {
        votes: 1,
        myVoteDir: -1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];
    const downVoteButton = voteButtons[1];

    expect(upVoteButton.classList).toContain("votable");
    expect(downVoteButton.classList).toContain("votable");
});

it("renders the downvote icon as highlighted when I downvoted before in edit mode", async () => {
    const props  = {
        votes: 1,
        myVoteDir: -1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const downVoteButton = voteButtons[1];

    expect(downVoteButton.classList).toContain("already-voted");
});

it("renders the icon as not highlighted in edit mode when I made no votes yet", async () => {
    const props  = {
        votes: 1,
        myVoteDir: 0
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];
    const downVoteButton = voteButtons[1];

    expect(upVoteButton.classList).not.toContain("already-voted");
    expect(downVoteButton.classList).not.toContain("already-voted");
});

it("increments the vote count by 1 when I upvote and have not voted before", async () => {
    const props  = {
        votes: 1,
        myVoteDir: 0
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];

    fireEvent.click(upVoteButton);

    const voteFields = document.getElementsByClassName("portfolio-row-votes-value")

    expect(voteFields.length).toEqual(1);
    await waitFor( () => expect(voteFields[0].innerHTML.trim()).toEqual("2"));
});

it("decrements the vote count by 1 when I downvote and have not voted before", async () => {
    const props  = {
        votes: 3,
        myVoteDir: 0
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const downVoteButton = voteButtons[1];

    fireEvent.click(downVoteButton);

    const voteFields = document.getElementsByClassName("portfolio-row-votes-value")

    expect(voteFields.length).toEqual(1);
    await waitFor( () => expect(voteFields[0].innerHTML.trim()).toEqual("2"));
});

it("decrements the vote count by 1 when I remove my upvote by clicking it again", async () => {
    const props  = {
        votes: 10,
        myVoteDir: 1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];

    fireEvent.click(upVoteButton);

    const voteFields = document.getElementsByClassName("portfolio-row-votes-value")

    expect(voteFields.length).toEqual(1);
    await waitFor(() => expect(voteFields[0].innerHTML.trim()).toEqual("9"));
});

it("increments the vote count by 1 when I remove my downvote by clicking it again", async () => {
    const props  = {
        votes: 10,
        myVoteDir: -1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const downVoteButton = voteButtons[1];

    fireEvent.click(downVoteButton);

    const voteFields = document.getElementsByClassName("portfolio-row-votes-value")

    expect(voteFields.length).toEqual(1);
    await waitFor( () => expect(voteFields[0].innerHTML.trim()).toEqual("11"));
});

it("increments the vote count by 2 when I switch from downvote to upvote", async () => {
    const props  = {
        votes: 10,
        myVoteDir: -1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={true}/></tr></tbody></table>);
    });
    
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];

    fireEvent.click(upVoteButton);

    const voteFields = document.getElementsByClassName("portfolio-row-votes-value")

    expect(voteFields.length).toEqual(1);
    await waitFor( () => expect(voteFields[0].innerHTML.trim()).toEqual("12"));
});

it("decrements the vote count by 2 when I switch from upvote to downvote", async () => {
    const props  = {
        votes: 10,
        myVoteDir: 1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn votes={props.votes} myVoteDir={props.myVoteDir} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const downVoteButton = voteButtons[1];

    fireEvent.click(downVoteButton);

    const voteFields = document.getElementsByClassName("portfolio-row-votes-value")

    expect(voteFields.length).toEqual(1);
    await waitFor( () => expect(voteFields[0].innerHTML.trim()).toEqual("8"));
});