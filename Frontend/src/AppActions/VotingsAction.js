import {
    VOTINGS_LOADING,
    VOTINGS_LOADED,
    VOTINGS_LOADING_ERROR,
    USER_TYPE
} from '../AppConstants/AppConstants';

export function VotingsLoaded(votingsResponse){
    return ({ type: VOTINGS_LOADED, payload: votingsResponse })
}

export function VotingsLoading(b){
    return ({ type: VOTINGS_LOADING, payload: b })
}

export function VotingsLoadingError(error) {
    return ({type: VOTINGS_LOADING_ERROR, payload: error})
}

export function LoadVotingsAsync(URL) {
    return async (dispatch) => {
        dispatch(VotingsLoading(true));
        let promise = fetch(URL);
        promise.then(response => response.json())
            .then(json => dispatch(VotingsLoaded(json)))
            .then(() => dispatch(VotingsLoading(false)))
            .catch((error) => dispatch(VotingsLoadingError(error)));
    }
}

export function ChangeUserType(type){
    return ({type: USER_TYPE, payload: type})

}