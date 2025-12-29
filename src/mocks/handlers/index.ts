import { authHandlers } from './auth'
import { articlesHandlers } from './articles'
import { documentsHandlers } from './documents'
import { notificationsHandlers } from './notifications'

export const handlers = [
  ...authHandlers,
  ...articlesHandlers,
  ...documentsHandlers,
  ...notificationsHandlers,
]
