import { screen, waitFor, within } from '@testing-library/react';

import MembersPage from './MembersPage';
import MockAdapter from 'axios-mock-adapter';
import MockFirebaseDB from '../../mocks/MockFirebaseDB';
import React from 'react';
import { act } from 'react-dom/test-utils';
import axios from 'axios';
import firebase from 'firebase/app';
import { render } from '../../test-util';

const testUrl = "http://localhost";

let mock;
let limit;
let offset;
let endPointUrl;

beforeEach(() => {
  window.HTMLElement.prototype.scrollIntoView = function() {};

  MockFirebaseDB.ServerValue =  { TIMESTAMP : 1000 };
  firebase.database = MockFirebaseDB;

  mock = new MockAdapter(axios);

  process.env = {
    REACT_APP_API_URL: testUrl,
    REACT_APP_API_MEMBER_LIMIT: 100
  };

  limit = 100;
  offset = 0;

  endPointUrl = `${process.env.REACT_APP_API_URL}/members?limit=${process.env.REACT_APP_API_MEMBER_LIMIT}&offset=${offset}`
});

afterEach(() => {
  mock.restore();
  process.env = {};
  firebase.database().reset();
});


it("can handle rendering initial empty state member data", async () => {

  const limit = 100;
  const offset = 0;

  const endPointUrl = `${process.env.REACT_APP_API_URL}/members?limit=${process.env.REACT_APP_API_MEMBER_LIMIT}&offset=${offset}`

  const memberData = {
    limit: limit,
    offset: offset,
    total: 0,
    memberGroups: {
    },
    uncategorizedMembers: []
  };

  mock.onGet(endPointUrl)
    .reply(200, memberData);

  act(() => {
    render(<MembersPage />);
  });

  Promise.resolve();

  await waitFor(() => expect(screen.getByRole("main")).toBeInTheDocument());

});


it("renders each member grouped by the first letter in their username when 1 user per grouping", async () => {

  const memberData = {
    limit: limit,
    offset: offset,
    total: 0,
    memberGroupings: {
      A: [
        {
          id: "100",
          name: "Abe"
        }
      ],
      B: [
        {
          id: "200",
          name: "Bart"
        }
      ]
    },
    uncategorizedMembers: []
  };

  mock.onGet(endPointUrl)
    .reply(200, memberData);

  act(() => {
    render(<MembersPage />);
  });

  Promise.resolve();

  await waitFor(() => expect(screen.queryAllByRole("list").length).toBe(2));

  const memberLists = screen.queryAllByRole("list");

  expect(memberLists.length).toEqual(2);

  expect(within(memberLists[0]).queryAllByRole("listitem").length).toEqual(1);
  expect(within(memberLists[0]).getByText("Abe")).toBeInTheDocument();

  expect(within(memberLists[1]).queryAllByRole("listitem").length).toEqual(1);
  expect(within(memberLists[1]).getByText("Bart")).toBeInTheDocument();
});

it("renders each member grouped by the first letter in their username when multiple user per grouping", async () => {

  const memberData = {
    limit: limit,
    offset: offset,
    total: 0,
    memberGroupings: {
      A: [
        {
          id: "100",
          name: "Abe"
        },
        {
          id: "101",
          name: "Arnold"
        }
      ],
      B: [
        {
          id: "200",
          name: "Bart"
        },
        {
          id: "201",
          name: "Ben"
        }
      ]
    },
    uncategorizedMembers: []
  };

  mock.onGet(endPointUrl)
    .reply(200, memberData);

  act(() => {
    render(<MembersPage />);
  });

  Promise.resolve();

  await waitFor(() => expect(screen.queryAllByRole("list").length).toBe(2));

  const memberLists = screen.queryAllByRole("list");

  expect(memberLists.length).toEqual(2);

  const a_members = within(memberLists[0]).queryAllByRole("listitem");
  const b_members = within(memberLists[1]).queryAllByRole("listitem");

  expect(a_members.length).toEqual(2);
  expect(within(a_members[0]).getByText("Abe")).toBeInTheDocument();
  expect(within(a_members[1]).getByText("Arnold")).toBeInTheDocument();

  expect(b_members.length).toEqual(2);
  expect(within(b_members[0]).getByText("Bart")).toBeInTheDocument();
  expect(within(b_members[1]).getByText("Ben")).toBeInTheDocument();
});

it("should ignore null or empty members", async () => {
  const memberData = {
    limit: limit,
    offset: offset,
    total: 0,
    memberGroupings: {
      A: [
        {
          id: "100",
          name: null
        }
      ],
      B: [
        {
          id: "200",
          name: ""
        }
      ]
    },
    uncategorizedMembers: []
  };

  mock.onGet(endPointUrl)
    .reply(200, memberData);

  act(() => {
    render(<MembersPage />);
  });

  Promise.resolve();

  await waitFor(() => expect(screen.getByRole("main")).toBeInTheDocument());

  expect(screen.queryAllByRole("list").length).toBe(0);
});

it("should skip headers for letters without members", async () => {

  const memberData = {
    limit: limit,
    offset: offset,
    total: 0,
    memberGroupings: {
      A: [
        {
          id: "100",
          name: "Abe"
        }
      ],
      Z: [
        {
          id: "200",
          name: "Zoro"
        }
      ]
    },
    uncategorizedMembers: []
  };

  mock.onGet(endPointUrl)
    .reply(200, memberData);

  act(() => {
    render(<MembersPage />);
  });

  Promise.resolve();

  await waitFor(() => expect(screen.queryAllByRole("list").length).toBe(2));

  const memberLists = screen.queryAllByRole("list");
  const headers = screen.queryAllByRole("heading");

  expect(memberLists.length).toEqual(2);

  expect(within(memberLists[0]).queryAllByRole("listitem").length).toEqual(1);
  expect(within(memberLists[0]).getByText("Abe")).toBeInTheDocument();

  expect(within(memberLists[1]).queryAllByRole("listitem").length).toEqual(1);
  expect(within(memberLists[1]).getByText("Zoro")).toBeInTheDocument();

  expect(headers.length).toEqual(3);
  expect(within(headers[0]).getByText("A")).toBeInTheDocument();
  expect(within(headers[1]).getByText("Z")).toBeInTheDocument();
  expect(within(headers[2]).getByText("Friend Activity")).toBeInTheDocument();
});

it("renders all members in a 'Others' category when the first character in their username is not a letter", async () => {
  const memberData = {
    limit: limit,
    offset: offset,
    total: 0,
    memberGroupings: {
      A: [
        {
          id: "100",
          name: "Abe"
        }
      ]
    },
    uncategorizedMembers: [{ id:"200", name: "carl"}]
  };

  mock.onGet(endPointUrl)
    .reply(200, memberData);

  act(() => {
    render(<MembersPage />);
  });

  Promise.resolve();

  await waitFor(() => expect(screen.queryAllByRole("list").length).toBe(2));

  const memberLists = screen.queryAllByRole("list");
  const headers = screen.queryAllByRole("heading");

  expect(memberLists.length).toEqual(2);

  expect(within(memberLists[0]).queryAllByRole("listitem").length).toEqual(1);
  expect(within(memberLists[0]).getByText("Abe")).toBeInTheDocument();

  expect(within(memberLists[1]).queryAllByRole("listitem").length).toEqual(1);
  expect(within(memberLists[1]).getByText("carl")).toBeInTheDocument();

  expect(headers.length).toEqual(3);
  expect(within(headers[0]).getByText("A")).toBeInTheDocument();
  expect(within(headers[1]).getByText("Others")).toBeInTheDocument();
  expect(within(headers[2]).getByText("Friend Activity")).toBeInTheDocument();
});