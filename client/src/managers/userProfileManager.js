//$ "create a manager function that we'll use in the WorkOrderList component to get all of the mechanics"



const _apiUrl = "/api/userprofile";

export const getUserProfiles = () => {
  return fetch(_apiUrl).then((res) => res.json());
};