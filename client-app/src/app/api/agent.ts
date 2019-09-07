import axios, { AxiosResponse } from 'axios'
import { IActivity, IActivitiesEnvelope } from '../models/activity';
import { history } from '../../index'
import { toast } from 'react-toastify'
import { IUser, IUserFormValues } from '../models/user'
import { IProfile, IPhoto } from '../models/profile';


axios.defaults.baseURL = process.env.REACT_APP_API_URL

axios.interceptors.request.use((config) => {
  const token = window.localStorage.getItem('jwt'); if (token) config.headers.Authorization = `Bearer ${token}`; return config
}, error => { return Promise.reject(error)})

axios.interceptors.response.use(undefined, error => {
  if (error.message === 'Network Error' && !error.response) toast.error('Network error - check that API is running!')
  const { status, config, data, headers } = error.response
  if (status === 401 && headers['www-authenticate'] === 'Bearer error="invalid_token", error_description="The token is expired"') {
    window.localStorage.removeItem('jwt'); history.push('/'); toast.info('Your session has expired, please login again')
  }
  if (status === 404) history.push('/notfound')
  if (status === 400 && config.method === 'get' && data.errors.hasOwnProperty('id')) history.push('/notfound')
  if (status === 500) toast.error('Server error - check the terminal for more info!')
  throw error.response
})

const responseBody = (response: AxiosResponse) => response.data

const requests = {
  get: (url: string) => axios.get(url).then(responseBody),
  post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
  put: (url: string, body: {}) => axios.put(url, body).then(responseBody),
  del: (url: string) => axios.delete(url).then(responseBody),
  postForm: (url: string, file: Blob) => { let formData = new FormData(); formData.append('File', file)
    return axios.post(url, formData, { headers: {'Content-Type': 'multipart/form-data'} }).then(responseBody) }
}

const Activities = {
  list: (params: URLSearchParams): Promise<IActivitiesEnvelope> => axios.get('/activities', {params: params}).then(responseBody),
  details: (id: string): Promise<IActivity> => requests.get(`/activities/${id}`),
  create: (activity: IActivity): Promise<IActivity> => requests.post('/activities', activity),
  update: (activity: IActivity): Promise<IActivity> => requests.put(`/activities/${activity.id}`, activity),
  delete: (id: string): Promise<void> => requests.del(`/activities/${id}`),
  attend: (id: string): Promise<void> => requests.post(`/activities/${id}/attend`, {}),
  unattend: (id: string): Promise<void> => requests.del(`/activities/${id}/attend`)
}

const User = {
  current: (): Promise<IUser> => requests.get('/user'),
  login: (user: IUserFormValues): Promise<IUser> => requests.post('/user/login', user),
  register: (user: IUserFormValues): Promise<IUser> => requests.post('/user/register', user)
}

const Profiles = {
  get: (username: string): Promise<IProfile> => requests.get(`/profiles/${username}`),
  updateProfile: (profile: Partial<IProfile>): Promise<void> => requests.put('/profiles', profile),
  uploadPhoto: (photo: Blob): Promise<IPhoto> => requests.postForm(`/photos`, photo),
  deletePhoto: (id: string): Promise<void> => requests.del(`/photos/${id}`),
  setMainPhoto: (id: string): Promise<void> => requests.post(`/photos/${id}/setmain`, {}),
  follow: (username: string) => requests.post(`/profiles/${username}/follow`, {}),
  unfollow: (username: string) => requests.del(`/profiles/${username}/follow`),
  listFollowings: (username: string, predicate: string) => requests.get(`/profiles/${username}/follow?predicate=${predicate}`),
  listActivities: (username: string, predicate: string) => requests.get(`/profiles/${username}/activities?predicate=${predicate}`)
}

export default { Activities, User, Profiles }