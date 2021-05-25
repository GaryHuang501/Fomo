const LeaderBoardOptions = {
    mostBullish:{
        headerClasses: "leader-board-positive-header",
        link: (id, name) => `${process.env.REACT_APP_CHART_URL}/${name}`
    },
    mostBearish:{
        headerClasses: "leader-board-negative-header",
        link: (id, name) => `${process.env.REACT_APP_CHART_URL}/${name}`
    },
    bestPerformers:{
        headerClasses: "leader-board-positive-header",
        link: (id, name) => `/portfolio/${id}`
    },
    worstPerformers:{
        headerClasses: "leader-board-negative-header",
        link: (id, name) => `/portfolio/${id}`
    }
}

export default LeaderBoardOptions;
