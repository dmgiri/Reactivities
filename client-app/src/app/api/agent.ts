import axios, { AxiosResponse } from 'axios'
import { IActivity } from '../models/activity'
import { history } from '../../index'
import { toast } from 'react-toastify'
import { IUser, IUserFormValues } from '../models/user'
import { IProfile, IPhoto } from '../models/profile';

axios.defaults.baseURL = 'http://localhost:5000/api'

axios.interceptors.request.use((config) => {
  const token = window.localStorage.getItem('jwt'); if (token) config.headers.Authorization = `Bearer ${token}`; return config
}, error => { return Promise.reject(error)})

axios.interceptors.response.use(undefined, error => {
  if (error.message === 'Network Error' && !error.response) toast.error('Network error - check that API is running!')
  const {status, config, data} = error.response
  if (status === 404) history.push('/notfound')
  if (status === 400 && config.method === 'get' && data.errors.hasOwnProperty('id')) history.push('/notfound')
  if (status === 500) toast.error('Server error - check the terminal for more info!')
  throw error.response
})

const responseBody = (response: AxiosResponse) => response.data
const sleep = (ms: number) => (response: AxiosResponse) => new Promise<AxiosResponse>(resolve => setTimeout(() => resolve(response), ms))

const requests = {
  get: (url: string) => axios.get(url).then(sleep(1000)).then(responseBody),
  post: (url: string, body: {}) => axios.post(url, body).then(sleep(1000)).then(responseBody),
  put: (url: string, body: {}) => axios.put(url, body).then(sleep(1000)).then(responseBody),
  del: (url: string) => axios.delete(url).then(sleep(1000)).then(responseBody),
  postForm: (url: string, file: Blob) => { let formData = new FormData(); formData.append('File', file)
    return axios.post(url, formData, { headers: {'Content-Type': 'multipart/form-data'} }).then(responseBody) }
}

const Activities = {
  list: (): Promise<IActivity[]> => requests.get('/activities'),
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
  uploadPhoto: (photo: Blob): Promise<IPhoto> => requests.postForm(`/photos`, photo),
  deletePhoto: (id: string): Promise<void> => requests.del(`/photos/${id}`),
  setMainPhoto: (id: string): Promise<void> => requests.post(`/photos/${id}/setmain`, {})
}

export default { Activities, User, Profiles }