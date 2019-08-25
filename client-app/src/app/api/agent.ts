import axios, { AxiosResponse } from 'axios'
import { IActivity } from '../models/activity'

axios.defaults.baseURL = 'http://localhost:5000/api'
const responseBody = (response: AxiosResponse) => response.data

const sleep = (ms: number) => (response: AxiosResponse) => new Promise<AxiosResponse>(resolve => setTimeout(() => resolve(response), ms))

const requests = {
  get: (url: string) => axios.get(url).then(sleep(1000)).then(responseBody),
  post: (url: string, body: {}) => axios.post(url, body).then(sleep(1000)).then(responseBody),
  put: (url: string, body: {}) => axios.put(url, body).then(sleep(1000)).then(responseBody),
  del: (url: string) => axios.delete(url).then(sleep(1000)).then(responseBody)
}

const Activities = {
  list: (): Promise<IActivity[]> => requests.get('/activities'),
  details: (id: string): Promise<IActivity> => requests.get(`/activities/${id}`),
  create: (activity: IActivity): Promise<IActivity> => requests.post('/activities', activity),
  update: (activity: IActivity): Promise<IActivity> => requests.put(`/activities/${activity.id}`, activity),
  delete: (id: string): Promise<void> => requests.del(`/activities/${id}`)
}

export default { Activities }