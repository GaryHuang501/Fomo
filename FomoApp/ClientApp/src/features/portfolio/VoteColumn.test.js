import { fireEvent, screen, waitFor } from '@testing-library/react';

import MockAdapter from 'axios-mock-adapter';
import MockFirebaseDB from '../../mocks/MockFirebaseDB';
import MockFirebaseRef from '../../mocks/MockFirebaseRef';
import { PortfolioStock } from './PortfolioStock';
import React from 'react';
import VoteColumn from './VoteColumn';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import firebase from 'firebase/app';
import { render } from '../../test-util';

const positiveDeltaClass = "positive-delta";
const negativeDeltaClass = "negative-delta";
const testUrl = "http://localhost";
let mock;

beforeEach(() => {
  MockFirebaseDB.ServerValue =  { TIMESTAMP : 1000 };
  firebase.database = MockFirebaseDB;

  mock = new MockAdapter(axios);

  process.env = {
    REACT_APP_API_URL: testUrl
  };

  mock.onPost(`${process.env.REACT_APP_API_URL}/votes`)
      .reply(200, {});
});

afterEach(() => {
  mock.restore();
  process.env = {};
  firebase.database().reset();
});


it("renders count count as 0 when no count found", async () => {

  const props  = {
    symbolId: 1
  }

  act(() => {
    render(<table><tbody><tr><VoteColumn {...props} isEditMode={false}/></tr></tbody></table>);
  });

  const voteFields = document.getElementsByClassName("portfolio-row-votes-value")

  expect(voteFields.length).toEqual(1);
  expect(voteFields[0].innerHTML.trim()).toEqual("0");
});

it("renders upvote button as green rocket ship when count > 0", async () => {
    const props  = {
        count: 1,
        myVoteDirection: 0,
        symbolId: 1
    };

    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={false}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];
    const downVoteButton = voteButtons[1];

    expect(upVoteButton.classList).toContain(positiveDeltaClass);
    expect(downVoteButton.classList).toContain("hidden");
});

it("renders downvote button as red rocket ship when count < 0", async () => {
    const props  = {
        count: -1,
        myVoteDirection: 0,
        symbolId: 1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={false}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];
    const downVoteButton = voteButtons[1];

    expect(downVoteButton.classList).toContain(negativeDeltaClass);
    expect(upVoteButton.classList).toContain("hidden");
});

it("renders both vote buttons normally when count is 0", async () => {
    const props  = {
        count: 0,
        myVoteDirection: 0,
        symbolId: 1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={false}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];
    const downVoteButton = voteButtons[1];

    expect(downVoteButton.classList).not.toContain("hidden");
    expect(upVoteButton.classList).not.toContain("hidden");
});

it("renders the upvote icon as highlighted when I upvoted before in edit mode", async () => {
    const props  = {
        count: 1,
        myVoteDirection: 1,
        symbolId: 1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];

    expect(upVoteButton.classList).toContain("already-voted");
});

it("renders the buttons as clickable when in edit mode", async () => {
    const props  = {
        count: 1,
        myVoteDirection: -1,
        symbolId: 1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];
    const downVoteButton = voteButtons[1];

    expect(upVoteButton.classList).toContain("votable");
    expect(downVoteButton.classList).toContain("votable");
});

it("renders the downvote icon as highlighted when I downvoted before in edit mode", async () => {
    const props  = {
        count: 1,
        myVoteDirection: -1,
        symbolId: 1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const downVoteButton = voteButtons[1];

    expect(downVoteButton.classList).toContain("already-voted");
});

it("renders the icon as not highlighted in edit mode when I made no count yet", async () => {
    const props  = {
        count: 1,
        myVoteDirection: 0,
        symbolId: 1
    };
    
    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];
    const downVoteButton = voteButtons[1];

    expect(upVoteButton.classList).not.toContain("already-voted");
    expect(downVoteButton.classList).not.toContain("already-voted");
});

it("increments the vote count by 1 when I upvote and have not voted before", async () => {
    const props  = {
        count: 1,
        myVoteDirection: 0,
        symbolId: 1
    };

    const spy = jest.spyOn(axios, 'post');
    
    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];

    fireEvent.click(upVoteButton);

    await Promise.resolve();
    await waitFor( () => expect(spy).toHaveBeenCalledWith(`${testUrl}/votes`, {symbolId: 1, direction: 1, delta: 1}));
});

it("decrements the vote count by 1 when I downvote and have not voted before", async () => {
    const props  = {
        count: 3,
        myVoteDirection: 0,
        symbolId: 1
    };
    
    const spy = jest.spyOn(axios, 'post');

    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const downVoteButton = voteButtons[1];

    fireEvent.click(downVoteButton);

    await Promise.resolve();
    await waitFor( () => expect(spy).toHaveBeenCalledWith(`${testUrl}/votes`, {symbolId: 1, direction: -1, delta: -1}));
});

it("decrements the vote count by 1 when I remove my upvote by clicking it again", async () => {
    const props  = {
        count: 10,
        myVoteDirection: 1,
        symbolId: 1
    };
    
    const spy = jest.spyOn(axios, 'post');

    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];

    fireEvent.click(upVoteButton);

    await Promise.resolve();
    await waitFor( () => expect(spy).toHaveBeenCalledWith(`${testUrl}/votes`, {symbolId: 1, direction: 0, delta: -1}));
});

it("increments the vote count by 1 when I remove my downvote by clicking it again", async () => {
    const props  = {
        count: 10,
        myVoteDirection: -1,
        symbolId: 1
    };

    const spy = jest.spyOn(axios, 'post');

    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const downVoteButton = voteButtons[1];

    fireEvent.click(downVoteButton);

    await Promise.resolve();
    await waitFor( () => expect(spy).toHaveBeenCalledWith(`${testUrl}/votes`, {symbolId: 1, direction: 0, delta: 1}));
});

it("increments the vote count by 2 when I switch from downvote to upvote", async () => {
    const props  = {
        symbolId: 1,
        count: 10,
        myVoteDirection: -1
    };

    const spy = jest.spyOn(axios, 'post');
    
    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>);
    });
    
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];

    fireEvent.click(upVoteButton);

    await Promise.resolve();
    await waitFor( () => expect(spy).toHaveBeenCalledWith(`${testUrl}/votes`, {symbolId: 1, direction: 1, delta: 2}));
});

it("decrements the vote count by 2 when I switch from upvote to downvote", async () => {
    const props  = {
        count: 10,
        myVoteDirection: 1,
        symbolId: 1
    };

    const spy = jest.spyOn(axios, 'post');
    
    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>);
    });
  
    const voteButtons = screen.queryAllByRole("button");
    const downVoteButton = voteButtons[1];

    fireEvent.click(downVoteButton);

    await Promise.resolve();
    await waitFor( () => expect(spy).toHaveBeenCalledWith(`${testUrl}/votes`, {symbolId: 1, direction: -1, delta: -2}));
});

describe("sends activity when voted", () =>{

  const props  = {
    count: 10,
    myVoteDirection: 0,
    symbolId: 1,
    ticker: "BAC"
  };
  
  const initialState = {
    login: {
      selectedUser: {
          id: "200",
          name: "myUser"
      },
      myUser: {
          id: "200",
          name: "myUser"
      }
    } 
  };

  it("sends activity for downvote", async () => {

    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>, {initialState});
    });
  
    const spy = jest.spyOn(axios, 'post');
  
    const voteButtons = screen.queryAllByRole("button");
    const upVoteButton = voteButtons[0];

    fireEvent.click(upVoteButton);

    await Promise.resolve();

    await waitFor( () => {
      const historyRef = firebase.database().refs.filter(r => r.path === 'friendActivity')[0];
      expect(historyRef).toBeDefined();
      expect(historyRef.messageQueue[0].text).toEqual("Upvoted BAC")
    });
  });

  it("sends activity for downvote", async () => {

    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>, {initialState});
    });
  
    const spy = jest.spyOn(axios, 'post');
  
    const voteButtons = screen.queryAllByRole("button");
    const downVoteButton = voteButtons[1];
    
    fireEvent.click(downVoteButton);

    await Promise.resolve();

    await waitFor( () => {
      const historyRef = firebase.database().refs.filter(r => r.path === 'friendActivity')[0];
      expect(historyRef).toBeDefined();
      expect(historyRef.messageQueue[0].text).toEqual("Downvoted BAC")
    });
  });

  it("sends activity for abstaining", async () => {

    props.myVoteDirection = -1;

    act(() => {
      render(<table><tbody><tr><VoteColumn {...props} isEditMode={true}/></tr></tbody></table>, {initialState});
    });
  
    const spy = jest.spyOn(axios, 'post');
  
    const voteButtons = screen.queryAllByRole("button");
    const downVoteButton = voteButtons[1];
    
    fireEvent.click(downVoteButton);

    await Promise.resolve();

    fireEvent.click(downVoteButton);

    await waitFor( () => {
      const historyRef = firebase.database().refs.filter(r => r.path === 'friendActivity')[0];
      expect(historyRef).toBeDefined();
      expect(historyRef.messageQueue[0].text).toEqual("Abstained BAC")
    });
  });
});

it("re-renders with new vote values when upvoted", async () => {
  const portfolioSymbol = { portfolioSymbolId: 1, symbolId: 1, ticker: 'abc' };

  const stockData = {
    symbolId: 1,
    ticker: 'SPY',
    price: 12.15,
    averagePrice: 12.50,
    changePercent: 12.50,
    return: 1
  };

  const voteData = {
    symbolId: 1,
    count: 9999,
    myVoteDirection: 0
  }

  const initialState = {
    stocks: {
      singleQuoteData:{
        1: stockData
      },
      votes:{
        1: voteData
      }
    }
  };

  act(() => {
    render(<table>
              <tbody>
                <PortfolioStock key={portfolioSymbol.symbolId} portfolioSymbol={portfolioSymbol} />
              </tbody>
            </table>, { initialState });
  });

  const voteButtons = screen.queryAllByRole("button").filter(b => b.classList.contains("fa-rocket"));
  const upvoteButton = voteButtons[0];

  fireEvent.click(upvoteButton);

  await waitFor( () => expect(screen.getByText(voteData.count + 1)).toBeInTheDocument());
});
