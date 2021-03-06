﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Codeping.Glutton.Core
{
    public class NotifyInfo
    {
        public NotifyInfo(UriNode node = null)
        {
            this.Node = node;
        }

        public string Message { get; internal set; }
        public bool HasException => this.RawException != null;
        public Exception RawException { get; internal set; }
        public MessageState State { get; set; }
        public UriNode Node { get; set; }

        public NotifyInfo Error(UriNode node, string error)
        {
            this.Node = node;

            this.Message = error;

            this.State = MessageState.Failed;

            return this;
        }

        public NotifyInfo Error(string error)
        {
            return this.Error(this.Node, error);
        }

        public NotifyInfo Error(UriNode node, Exception exception)
        {
            this.Node = node;

            this.RawException = exception;

            this.Message = exception.Message;

            this.State = MessageState.Failed;

            return this;
        }

        public NotifyInfo Error(Exception exception)
        {
            return this.Error(this.Node, exception);
        }

        public NotifyInfo Skiped(UriNode node)
        {
            this.Node = node;

            this.State = MessageState.Skiped;

            return this;
        }

        public NotifyInfo Skiped()
        {
            return Skiped(this.Node);
        }

        public NotifyInfo Downloading(UriNode node)
        {
            this.Node = node;

            this.State = MessageState.Downloading;

            return this;
        }

        public NotifyInfo Downloading()
        {
            return this.Downloading(this.Node);
        }

        public NotifyInfo Downloaded(UriNode node)
        {
            this.Node = node;

            this.State = MessageState.Downloaded;

            return this;
        }

        public NotifyInfo Downloaded()
        {
            return this.Downloaded(this.Node);
        }
    }

    public enum MessageState
    {
        Downloading,
        Downloaded,
        Failed,
        Skiped,
    }
}
